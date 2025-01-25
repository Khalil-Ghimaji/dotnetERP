using Persistence.entities.Commande;

namespace Persistence.Repository.CommandeRepositories;

public class ArticleCommandeRepo : GenericRepository<ArticleCommande>,IArticleCommandeRepo
{
    public ArticleCommandeRepo(AppDbContext context) : base(context)
    { }
}