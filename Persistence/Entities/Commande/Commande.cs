using Persistence.entities.Stock;

namespace Persistence.entities.Commande;

public class Commande
{
    public int Id { get; set; }
    public ICollection<ArticleCommande> articles { get; set; }
}