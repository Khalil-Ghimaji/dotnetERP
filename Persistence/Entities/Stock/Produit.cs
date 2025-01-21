using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.entities.Stock;

public class Produit : Common
{
    public string Nom { get; set; }
    public double Prix { get; set; }
    public int CategorieId { get; set; }
    [ForeignKey("CategorieId")] public Categorie Categorie { get; set; }
    
    public ArticleStock ArticleStock { get; set; }
}