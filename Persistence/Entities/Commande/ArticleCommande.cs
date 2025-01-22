using Persistence.entities.Stock;

namespace Persistence.entities.Commande;

public record ArticleCommande
{
    public int id { get; set; }
    public Commande commande { get; set; }
    public Produit produit { get; set; }
    public int quantite { get; set; }
    public double prix { get; set; }
}