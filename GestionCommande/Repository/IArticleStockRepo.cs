using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionCommande.Repository;

public interface IArticleStockRepo : IGenericRepository<ArticleStock>
{
    public double getPrixArticle(int produitId);
}