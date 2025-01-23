using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Persistence.entities.Stock;

[Index(nameof(Nom), IsUnique = true)]
public class Produit
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public int CategorieId { get; set; }
    [ForeignKey("CategorieId")] 
    public Categorie Categorie { get; set; }

    public virtual ArticleStock ArticleStock { get; set; }
}