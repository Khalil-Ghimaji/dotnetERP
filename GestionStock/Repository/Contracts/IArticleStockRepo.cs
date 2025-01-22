using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public interface IStockRepo:IGenericRepository<ArticleStock>
{
    public Task<ArticleStock?> GetArticleStockByProduitId(int produitId);
}