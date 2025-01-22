using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Persistence.entities.Stock;

[PrimaryKey(nameof(ProduitId))]
public class ArticleStock
{
    public int ProduitId { get; set; }
    [Range(0, double.MaxValue)] public double Prix { get; set; }
    [ForeignKey("ProduitId")] public Produit Produit { get; set; }
    [Required] [Range(0, int.MaxValue)] public int Quantite { get; set; }
}