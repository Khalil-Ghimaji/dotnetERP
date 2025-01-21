using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.entities.Stock;

public class ArticleStock : Common
{
    public Guid ProduitId { get; set; }
    [ForeignKey("ProduitId")] public Produit Produit { get; set; }
    [Required] [Range(0, int.MaxValue)] public int Quantite { get; set; }
}