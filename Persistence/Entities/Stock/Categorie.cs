namespace Persistence.entities.Stock;

public class Categorie
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Description { get; set; }
    public ICollection<Produit> Produits { get; set; }
}