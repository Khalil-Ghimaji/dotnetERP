using System.Collections.Concurrent;
using System.Data;
using AutoMapper;
using GestionStock.DTO;
using AjouterQuantiteRequestDTO = GestionStock.DTO.ArticleExpedierMarchandisesDTO;
using Persistence;
using Persistence.entities.Stock;
using Persistence.Repository.StockRepositories.Contracts;

namespace GestionStock.Services
{
    public class StockService : IStockService
    {
        private readonly IArticleStockRepo _stockRepo;
        private readonly IMapper _mapper;
        private readonly IProduitRepo _produitRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly AppDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly
            ConcurrentDictionary<(Guid reservationId, int produitId), (CancellationTokenSource cts, int quantite)>
            _reservationTasks =
                new ConcurrentDictionary<(Guid, int), (CancellationTokenSource, int)>();

        public StockService(IArticleStockRepo stockRepo, IMapper mapper, IProduitRepo produitRepo,
            IServiceScopeFactory scopeFactory,
            ICategoryRepo categoryRepo,
            AppDbContext context)
        {
            _stockRepo = stockRepo;
            _mapper = mapper;
            _scopeFactory = scopeFactory;
            _produitRepo = produitRepo;
            _categoryRepo = categoryRepo;
            _context = context;
        }


        //permet d'ajouter un produit avec son stock associé
        public async Task<int> AjouterProduit(AjouterProduitRequestDTO dto)
        {
            //check if the product already exists
            if (await _produitRepo.ProduitExists(dto.Nom))
            {
                throw new DuplicateNameException("Produit déjà existant.");
            }

            //check if the category exists
            if (!await _categoryRepo.CategoryExists(dto.CategoryId))
            {
                throw new KeyNotFoundException("Catégorie non trouvée.");
            }

            //créer le produit avec l'ArticleStock
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


        //reste à remplacer Commande par un DTO
        public async Task ExpedierMarchandises(ExpedierMarchandisesRequestDTO commande)
        {
            foreach (var item in commande.Articles)
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
        }


        //cette méthode met à jour les informations d'un produit et de son stock
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


        public async Task<Guid> ReserverProduit(ReserverProduitRequestDTO dto)
        {
            var articleStock = await _stockRepo.GetArticleStockByProduitId(dto.ProduitId);
            if (articleStock != null && articleStock.Quantite >= dto.Quantite)
            {
                articleStock.Quantite -= dto.Quantite;
                await _stockRepo.Update(articleStock);

                var reservationId = Guid.NewGuid();
                var cts = new CancellationTokenSource();
                _reservationTasks[(reservationId, dto.ProduitId)] = (cts, dto.Quantite);

                if (TimeSpan.TryParse(dto.ReservationDuration, out var reservationDuration))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(reservationDuration, cts.Token);
                            cts.Token.ThrowIfCancellationRequested();
                            using (var scope = _scopeFactory.CreateScope())
                            {
                                var scopedStockRepo = scope.ServiceProvider.GetRequiredService<IArticleStockRepo>();
                                articleStock.Quantite += dto.Quantite;
                                await scopedStockRepo.Update(articleStock);
                            }
                        }
                        catch (TaskCanceledException)
                        {
                        }
                        finally
                        {
                            _reservationTasks.TryRemove((reservationId, dto.ProduitId), out _);
                        }
                    });
                }
                else
                {
                    throw new InvalidOperationException("Timespan invalide.");
                }

                return reservationId;
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

        public async Task AnnulerCommande(Guid id)
        {
            var reservation = _reservationTasks.FirstOrDefault(r => r.Key.reservationId == id);
            if (reservation.Key != default)
            {
                var (reservationId, produitId) = reservation.Key;
                var (cts, quantite) = reservation.Value;

                cts.Cancel();
                _reservationTasks.TryRemove((reservationId, produitId), out _);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedStockRepo = scope.ServiceProvider.GetRequiredService<IArticleStockRepo>();
                    var articleStock = await scopedStockRepo.GetArticleStockByProduitId(produitId);
                    if (articleStock != null)
                    {
                        articleStock.Quantite += quantite;
                        await scopedStockRepo.Update(articleStock);
                    }
                }
            }
            else
            {
                throw new KeyNotFoundException("Reservation non trouvée.");
            }
        }

        public void ConfirmerCommande(Guid id)
        {
            var reservation = _reservationTasks.FirstOrDefault(r => r.Key.reservationId == id);
            if (reservation.Key != default)
            {
                var (reservationId, produitId) = reservation.Key;
                var (cts, _) = reservation.Value;

                cts.Cancel();
                _reservationTasks.TryRemove((reservationId, produitId), out _);
            }
            else
            {
                throw new KeyNotFoundException("Reservation non trouvée.");
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
                //propager la même exception
                throw;
            }
        }
    }
}