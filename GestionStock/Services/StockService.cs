using System.Collections.Concurrent;
using System.Data;
using AutoMapper;
using GestionStock.DTO;
using AjouterQuantiteRequestDTO = GestionStock.DTO.ArticleExpedierMarchandisesDTO;
using Persistence;
using Persistence.entities.Commande;
using Persistence.entities.Stock;
using Persistence.Repository.CommandeRepositories;
using Persistence.Repository.StockRepositories.Contracts;

namespace GestionStock.Services
{
    public class StockService : IStockService
    {
        private readonly IArticleStockRepo _stockRepo;
        private readonly IMapper _mapper;
        private readonly IProduitRepo _produitRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly ICommandeRepo _commandeRepo;
        private readonly AppDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly ConcurrentDictionary<int, CancellationTokenSource> _reservationTasks =
            new ConcurrentDictionary<int, CancellationTokenSource>();

        public StockService(IArticleStockRepo stockRepo, IMapper mapper, IProduitRepo produitRepo,
            IServiceScopeFactory scopeFactory,
            ICategoryRepo categoryRepo,
            ICommandeRepo commandeRepo,
            AppDbContext context)
        {
            _stockRepo = stockRepo;
            _commandeRepo = commandeRepo;
            _mapper = mapper;
            _scopeFactory = scopeFactory;
            _produitRepo = produitRepo;
            _categoryRepo = categoryRepo;
            _context = context;
        }

        public async Task<int> AjouterProduit(AjouterProduitRequestDTO dto)
        {

            if (await _produitRepo.ProduitExists(dto.Nom))
            {
                throw new DuplicateNameException("Produit déjà existant.");
            }

            if (!await _categoryRepo.CategoryExists(dto.CategoryId))
            {
                throw new KeyNotFoundException("Catégorie non trouvée.");
            }

            var produit = new Produit()
            {
                Nom = dto.Nom,
                CategorieId = dto.CategoryId
            };
            await _produitRepo.Add(produit);

            var articleStock = new ArticleStock()
            {
                Prix = (dto.Prix >= 0) ? dto.Prix : 0,
                Quantite = (dto.Quantite >= 0) ? dto.Quantite : 0,
                ProduitId = produit.Id,
            };
            await _stockRepo.Add(articleStock);
            return produit.Id;
        }

        public async Task<ArticleStockDTO> ConsulterProduit(int id)
        {
            var articleStock = await _stockRepo.GetArticleStockByProduitId(id);
            if (articleStock != null)
            {
                return _mapper.Map<ArticleStockDTO>(articleStock);
            }

            throw new KeyNotFoundException("Article non trouvé.");
        }

        public async Task AjouterQuantite(AjouterQuantiteRequestDTO dto)
        {
            var articleStock = await _stockRepo.GetArticleStockByProduitId(dto.ProduitId);
            if (articleStock != null)
            {
                articleStock.Quantite += dto.Quantite;
                await _stockRepo.Update(articleStock);
            }
            else
            {
                throw new KeyNotFoundException("Article non trouvé.");
            }
        }


        public async Task<IEnumerable<ArticleStockDTO>> ConsulterStock()
        {
            var articleStocks = await _stockRepo.GetAll();
            return _mapper.Map<IEnumerable<ArticleStockDTO>>(articleStocks);
        }

        public async Task ExpedierMarchandises(int idCommande)
        {
            var commande = await _commandeRepo.GetById(idCommande);
            if (commande == null)
            {
                throw new KeyNotFoundException("commande non trouvee");
            }

            var articles = _mapper.Map<IEnumerable<ArticleExpedierMarchandisesDTO>>(commande.articles);
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in articles)
                {
                    var articleStock = await _stockRepo.GetArticleStockByProduitId(item.ProduitId);
                    if (articleStock == null)
                    {
                        throw new KeyNotFoundException("Article non trouvé.");
                    }

                    if (articleStock.Quantite < item.Quantite)
                    {
                        throw new InvalidOperationException("Quantité insuffisante.");
                    }

                    articleStock.Quantite -= item.Quantite;
                    await _stockRepo.Update(articleStock);
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ModifierProduit(ProduitDTO dto)
        {
            var articleStock = await _stockRepo.GetArticleStockByProduitId(dto.ProduitId);
            if (articleStock != null)
            {
                if (dto.Quantite > 0)
                    articleStock.Quantite = dto.Quantite;
                if (dto.Prix > 0)
                    articleStock.Prix = dto.Prix;
                if (dto.CategoryId > 0 && await _categoryRepo.CategoryExists(dto.CategoryId))
                    articleStock.Produit.CategorieId = dto.CategoryId;
                if (!string.IsNullOrEmpty(dto.Nom))
                    articleStock.Produit.Nom = dto.Nom;
                await _stockRepo.Update(articleStock);
            }
            else
            {
                throw new KeyNotFoundException("Article non trouvé.");
            }
        }

        public async Task ReserverCommande(ReserverCommandeRequestDTO reserverCommande)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var commande = await _commandeRepo.GetById(reserverCommande.idCommande);
                if (commande == null)
                {
                    throw new KeyNotFoundException("Commande non trouvée.");
                }

                var articles = commande.articles;
                foreach (var article in articles)
                {
                    var articleStock = article.produit.ArticleStock;
                    if (articleStock.Quantite >= article.quantite)
                    {
                        articleStock.Quantite -= article.quantite;
                        await _stockRepo.Update(articleStock);
                    }
                    else
                    {
                        if (articleStock == null)
                        {
                            throw new KeyNotFoundException("Article non trouvé.");
                        }

                        throw new InvalidOperationException("Quantité insuffisante");
                    }
                }

                var cts = new CancellationTokenSource();
                _reservationTasks[reserverCommande.idCommande] = cts;
                if (TimeSpan.TryParse(reserverCommande.ReservationDuration, out var reservationDuration))
                {
                    _ = Task.Run(async () =>
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var scopedStockRepo = scope.ServiceProvider.GetRequiredService<IArticleStockRepo>();
                            var scopedCommandeRepo = scope.ServiceProvider.GetRequiredService<ICommandeRepo>();

                            try
                            {
                                await Task.Delay(reservationDuration, cts.Token);
                                cts.Token.ThrowIfCancellationRequested();
                                commande = await scopedCommandeRepo.GetById(reserverCommande.idCommande);
                                commande.status = StatusCommande.FACTUREE;
                                foreach (var article in articles)
                                {
                                    var articleStock =
                                        await scopedStockRepo.GetArticleStockByProduitId(article.produit.Id);
                                    if (articleStock != null)
                                    {
                                        articleStock.Quantite += article.quantite;
                                        await scopedStockRepo.Update(articleStock);
                                    }
                                }

                                await scopedCommandeRepo.Update(commande);
                            }
                            catch (TaskCanceledException)
                            {
                            }
                            finally
                            {
                                _reservationTasks.TryRemove(reserverCommande.idCommande, out _);
                            }
                        }
                    });
                }
                else
                {
                    throw new InvalidOperationException("Timespan invalide.");
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AnnulerCommande(int CommandeId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var reservation
                    = _reservationTasks.FirstOrDefault(x => x.Key == CommandeId);

                if (reservation.Key != default)
                {
                    var commandeId = reservation.Key;
                    var cts = reservation.Value;

                    cts.Cancel();
                    _reservationTasks.TryRemove(commandeId, out _);
                    var commande = await _commandeRepo.GetById(commandeId);
                    var articles = commande?.articles;
                    foreach (var article in articles)
                    {
                        var articleStock = await _stockRepo.GetArticleStockByProduitId(article.produit.Id);
                        if (articleStock != null)
                        {
                            articleStock.Quantite += article.quantite;
                            await _stockRepo.Update(articleStock);
                        }
                        else
                        {
                            throw new KeyNotFoundException("Article non trouvé.");
                        }
                    }
                }
                else
                {
                    throw new KeyNotFoundException("Reservation non trouvée.");
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ConfirmerCommande(int CommandeId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var reservations = _reservationTasks.Where(x => x.Key == CommandeId).ToList();
                foreach (var reservation in reservations)
                {
                    if (reservation.Key != default)
                    {
                        var commandeId = reservation.Key;
                        var cts = reservation.Value;

                        cts.Cancel();
                        _reservationTasks.TryRemove(commandeId, out _);
                    }
                    else
                    {
                        throw new KeyNotFoundException("Reservation non trouvée.");
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SupprimerProduit(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _stockRepo.Delete(id) == null || await _produitRepo.Delete(id) == null)
                {
                    throw new HttpRequestException("Article non trouvé.");
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}