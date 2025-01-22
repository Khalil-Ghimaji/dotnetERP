using System.Collections.Concurrent;
using AutoMapper;
using GestionStock.DTO;
using GestionStock.Repository;
using Persistence.entities.Commande;
using Persistence.entities.Stock;

namespace GestionStock.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepo _stockRepo;
        private readonly IMapper _mapper;
        private readonly ConcurrentDictionary<int, TaskCompletionSource<bool>> _reservationTasks;

        public StockService(IStockRepo stockRepo, IMapper mapper)
        {
            _stockRepo = stockRepo;
            _mapper = mapper;
            _reservationTasks = new ConcurrentDictionary<int, TaskCompletionSource<bool>>();
        }

        public async Task CreerProduit()
        {
            
        }
        
        public async Task AjouterProduit(CreerProduitDTO dto)
        {
            var produit = _mapper.Map<Produit>(dto);
            var articleStock = await _stockRepo.GetByProduitId(produit.Id);
            if (articleStock == null)
            {
                _stockRepo.AddArticleStock(produit.Id, dto.Quantite);
            }
            else
            {
                articleStock.Quantite += dto.Quantite;
                await _stockRepo.Update(articleStock);
            }
        }

        public async Task<IEnumerable<ArticleStockDTO>> ConsulterStock()
        {
            var articleStocks = await _stockRepo.GetAll();
            return _mapper.Map<IEnumerable<ArticleStockDTO>>(articleStocks);
        }

        public async Task ExpedierMarchandises(Commande commande)
        {
            foreach (var item in commande.ArticleCommandes)
            {
                var articleStock = await _stockRepo.GetByProduitId(item.ProduitId);
                if (articleStock != null)
                {
                    articleStock.Quantite -= item.Quantite;
                    await _stockRepo.Update(articleStock);
                }
            }
        }

        public async Task ModifierProduit(ModifierProduitDTO dto)
        {
            var articleStock = await _stockRepo.GetByProduitId(dto.ProduitId);
            if (articleStock != null)
            {
                if(dto.Quantite >= 0)
                    articleStock.Quantite = dto.Quantite;
                if(dto.CategoryId >0)
                    articleStock.Produit.CategorieId = dto.CategoryId;
                if(dto.Nom != String.Empty)
                    articleStock.Produit.Nom = dto.Nom;
                await _stockRepo.Update(articleStock);
            }
        }

        public async Task ReserverProduit(ReserverProduitDTO dto)
        {
            var articleStock = await _stockRepo.GetByProduitId(dto.ProduitId);
            if (articleStock != null && articleStock.Quantite >= dto.Quantite)
            {
                articleStock.Quantite -= dto.Quantite;
                await _stockRepo.Update(articleStock);

                var tcs = new TaskCompletionSource<bool>();
                _reservationTasks[dto.ProduitId] = tcs;

                var delayTask = Task.Delay(dto.ReservationDuration);
                var completedTask = await Task.WhenAny(delayTask, tcs.Task);

                if (completedTask == delayTask)
                {
                    articleStock.Quantite += dto.Quantite;
                    await _stockRepo.Update(articleStock);
                }

                _reservationTasks.TryRemove(dto.ProduitId, out _);
            }
            else
            {
                throw new ArgumentException("Quantité insuffisante ou article non trouvé.");
            }
        }

        public void ConfirmerCommande(int id)
        {
            if (_reservationTasks.TryGetValue(id, out var tcs))
            {
                tcs.SetResult(true);
            }
        }

        public async Task SupprimerProduit(int id)
        {
            await _stockRepo.Delete(id);
        }
    }
}