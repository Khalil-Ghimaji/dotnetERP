using Persistence;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionCommande.Repository;

public class ArticleStockRepo : GenericRepository<ArticleStock>, IArticleStockRepo
{
    public ArticleStockRepo(AppDbContext context) : base(context) { }

    public double getPrixArticle(int produitId)
    {
        return _dbSet.FirstOrDefault(a => a.produitId == produitId)?.prix??0;
    }
}