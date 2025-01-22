using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public interface IStockRepo:IGenericRepository<ArticleStock>
{
    public Task<Produit?> GetProduitById(int produitId);
    public Task<ArticleStock?> GetByProduitId(int produitId);
    public void AddArticleStock(int productId, int quantite,double prix);
}