namespace Persistence.entities.Stock;

public class ArticleStock:Common
{
    public int Id { get; set; }
    public int produitId { get; set; }
    public Produit produit { get; set; }
    public double prix { get; set; }
    public int quantite { get; set; }
}