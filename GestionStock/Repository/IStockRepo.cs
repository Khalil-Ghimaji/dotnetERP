using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public interface IStockRepo:IGenericRepository<ArticleStock>
{
    public Task<Produit?> GetProduitById(Guid produitId);
    public Task<ArticleStock?> GetByProduitId(Guid produitId);
    public void AddArticleStock(Guid productId, int quantite);
}