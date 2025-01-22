using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.entities.Stock;

public class ArticleStock : Common
{
    public int ProduitId { get; set; }
    public double Prix { get; set; }
    public Produit Produit { get; set; }
    [Required] [Range(0, int.MaxValue)] public int Quantite { get; set; }
}