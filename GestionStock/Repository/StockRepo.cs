using Persistence;
using Persistence.entities.Stock;
using Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Repository
{
    public class StockRepo : GenericRepository<ArticleStock>, IStockRepo
    {
        public StockRepo(AppDbContext context) : base(context)
        {
        }
        public async Task<Produit?> GetProduitById(int produitId)
        {
            return await _context.Produits.FindAsync(produitId);
        }

        public async Task<ArticleStock?> GetByProduitId(int produitId)
        {
            return await _context.AricleStocks.FirstOrDefaultAsync(a => a.ProduitId == produitId);
        }
        public async void AddArticleStock(int productId, int quantite)
        {
            ArticleStock articleStock = new ArticleStock
            {
                ProduitId = productId,
                Quantite = quantite
            };
            await Add(articleStock);
        }
    }
}