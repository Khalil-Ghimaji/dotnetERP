using Persistence.entities.Stock;

namespace Persistence.entities.Commande;

public class Commande:Common
{
    public ICollection<ArticleCommande> articles{ get; set; }
    
}