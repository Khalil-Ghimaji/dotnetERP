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
        public async Task<ArticleStock?> GetArticleStockByProduitId(int produitId)
        {
            return await _context.AricleStocks.FirstOrDefaultAsync(a => a.ProduitId == produitId);
        }
    }
}