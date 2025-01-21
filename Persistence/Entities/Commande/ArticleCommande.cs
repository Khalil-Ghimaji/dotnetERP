using System.ComponentModel.DataAnnotations.Schema;
using Persistence.entities.Stock;

namespace Persistence.entities.Commande;

public class ArticleCommande:Common
{
    public Guid ProduitId { get; set; }
    public int Quantite { get; set; }
    [ForeignKey("ProduitId")]
    public Produit Produit { get; set; }
}