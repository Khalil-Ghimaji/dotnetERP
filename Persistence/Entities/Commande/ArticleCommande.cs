using System.ComponentModel.DataAnnotations.Schema;
using Persistence.entities.Stock;

namespace Persistence.entities.Commande;

public class ArticleCommande
{
    public int Id { get; set; }
    public int ProduitId { get; set; }
    public int Quantite { get; set; }
    [ForeignKey("ProduitId")]
    public virtual Produit Produit { get; set; }
}