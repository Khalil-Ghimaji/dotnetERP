using Persistence.entities.Stock;

namespace Persistence.entities.Commande;

public record ArticleCommande
{
    public int Id { get; set; }
    public virtual Commande commande { get; set; }
    public virtual Produit produit { get; set; }
    public int quantite { get; set; }
    public double prix { get; set; }
}