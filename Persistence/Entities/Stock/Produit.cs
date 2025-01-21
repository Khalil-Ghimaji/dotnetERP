using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.entities.Stock;

public class Produit : Common
{
    public string Nom { get; set; }
    public double Prix { get; set; }
    public Guid CategorieId { get; set; }
    [ForeignKey("CategorieId")] public Categorie Categorie { get; set; }

    public Guid ArticleStockId { get; set; }
    [ForeignKey("ArticleStockId")] public ArticleStock ArticleStock { get; set; }
}