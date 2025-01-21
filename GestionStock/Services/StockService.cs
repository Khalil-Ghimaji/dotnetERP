using System.Collections.Concurrent;
using GestionStock.Repository;
using Persistence.entities.Commande;
using Persistence.entities.Stock;

namespace GestionStock.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepo _stockRepo;
        private readonly ConcurrentDictionary<int, TaskCompletionSource<bool>> _reservationTasks;

        public StockService(IStockRepo stockRepo)
        {
            _stockRepo = stockRepo;
            _reservationTasks = new ConcurrentDictionary<int, TaskCompletionSource<bool>>();
        }

        public async void AjouterProduit(int produitId, int quantite)
        {
            var produit = await _stockRepo.GetProduitById(produitId);
            if (produit == null)
            {
                throw new ArgumentException("Le produit n'existe pas.");
            }

            var articleStock = await _stockRepo.GetByProduitId(produitId);
            if (articleStock == null)
            {
                _stockRepo.AddArticleStock(produitId, quantite);
            }
            else
            {
                articleStock.Quantite += quantite;
                await _stockRepo.Update(articleStock);
            }
        }

        public async Task<IEnumerable<ArticleStock>> ConsulterStock()
        {
            return await _stockRepo.GetAll();
        }

        public async Task ExpedierMarchandises(Commande commande)
        {
            foreach (var item in commande.ArticleCommandes)
            {
                var articleStock = await _stockRepo.GetById(item.ProduitId);
                if (articleStock != null)
                {
                    articleStock.Quantite -= item.Quantite;
                    await _stockRepo.Update(articleStock);
                }
            }
        }

        public async Task ModifierProduit(int id, int quantite)
        {
            var articleStock = await _stockRepo.GetByProduitId(id);
            if (articleStock != null)
            {
                articleStock.Quantite = quantite;
                await _stockRepo.Update(articleStock);
            }
        }

        public async Task ReserverProduit(int id, int quantite, TimeSpan reservationDuration)
        {
            var articleStock = await _stockRepo.GetByProduitId(id);
            if (articleStock != null && articleStock.Quantite >= quantite)
            {
                articleStock.Quantite -= quantite;
                await _stockRepo.Update(articleStock);

                var tcs = new TaskCompletionSource<bool>();
                _reservationTasks[id] = tcs;

                var delayTask = Task.Delay(reservationDuration);
                var completedTask = await Task.WhenAny(delayTask, tcs.Task);

                if (completedTask == delayTask)
                {
                    articleStock.Quantite += quantite;
                    await _stockRepo.Update(articleStock);
                }

                _reservationTasks.TryRemove(id, out _);
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