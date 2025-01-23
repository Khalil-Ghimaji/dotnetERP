using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<Guid, CancellationTokenSource> _reservationTasks=new ConcurrentDictionary<Guid, CancellationTokenSource>();

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
                throw new HttpRequestException("Produit déjà existant.");
            }

            //check if the category exists
            if (!await _categoryRepo.CategoryExists(dto.CategoryId))
            {
                throw new HttpRequestException("Catégorie non trouvée.");
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

            throw new HttpRequestException("Article non trouvé.");
        }

        public async Task AjouterQuantite(AjouterQuantiteRequestDTO dto)
        {
            Console.WriteLine("Entering AjouterQuantite method.");
            var articleStock = await _stockRepo.GetArticleStockByProduitId(dto.ProduitId);
            if (articleStock != null)
            {
                articleStock.Quantite += dto.Quantite;
                await _stockRepo.Update(articleStock);
                Console.WriteLine("Quantity updated successfully.");
            }
            else
            {
                Console.WriteLine("Article not found.");
                throw new HttpRequestException("Article non trouvé.");
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
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in commande.Articles)
                {
                    var articleStock = await _stockRepo.GetArticleStockByProduitId(item.ProduitId);
                    if (articleStock == null || articleStock.Quantite < item.Quantite)
                    {
                        throw new HttpRequestException("Quantité insuffisante ou article non trouvé.");
                    }

                    articleStock.Quantite -= item.Quantite;
                    await _stockRepo.Update(articleStock);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                //propager la même exception
                throw;
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
                throw new HttpRequestException("Article non trouvé.");
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
                _reservationTasks[reservationId] = cts;

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
                            Console.WriteLine($"Task canceled for reservationId: {reservationId}");
                        }
                        finally
                        {
                            _reservationTasks.TryRemove(reservationId, out _);
                        }
                    });
                }
                else
                {
                    throw new ArgumentException("Invalid TimeSpan format.");
                }

                return reservationId;
            }
            else
            {
                throw new ArgumentException("Quantité insuffisante ou article non trouvé.");
            }
        }

        public void ConfirmerCommande(Guid id)
        {
            if (_reservationTasks.TryGetValue(id, out var cts))
            {
                cts.Cancel();
                _reservationTasks.TryRemove(id, out _);
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