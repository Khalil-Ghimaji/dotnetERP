using Microsoft.EntityFrameworkCore;
using Persistence.entities.Stock;
using Persistence.Repository.StockRepositories.Contracts;

namespace Persistence.Repository.StockRepositories.Implementations
{
    public class ArticleStockRepo : GenericRepository<ArticleStock>, IArticleStockRepo
    {
        public ArticleStockRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<ArticleStock?> GetArticleStockByProduitId(int produitId)
        {
            return await _context.AricleStocks.FirstOrDefaultAsync(a => a.ProduitId == produitId);
        }
        
        public double getPrixArticle(int produitId)
        {
            return _dbSet.FirstOrDefault(a => a.ProduitId == produitId)?.Prix??0;
        }
    }
}