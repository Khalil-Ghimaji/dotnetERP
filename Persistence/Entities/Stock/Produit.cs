using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Persistence.entities.Stock;

public class Produit
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public int CategorieId { get; set; }
    [ForeignKey("CategorieId")] 
    public virtual Categorie Categorie { get; set; }

    public virtual ArticleStock ArticleStock { get; set; }
}