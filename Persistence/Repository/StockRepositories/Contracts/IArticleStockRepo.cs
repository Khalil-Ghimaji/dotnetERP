using Persistence.entities.Stock;

namespace Persistence.Repository.StockRepositories.Contracts;

public interface IArticleStockRepo:IGenericRepository<ArticleStock>
{
    public Task<ArticleStock?> GetArticleStockByProduitId(int produitId);
    public double getPrixArticle(int produitId);
}