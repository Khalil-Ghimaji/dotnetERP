using System.ComponentModel.DataAnnotations;

namespace Persistence.entities.Stock;

public class ArticleStock
{
    public int Id { get; set; }
    public int ProduitId { get; set; }
    public double Prix { get; set; }
    public Produit Produit { get; set; }
    [Required] 
    [Range(0, int.MaxValue)] 
    public int Quantite { get; set; }
}