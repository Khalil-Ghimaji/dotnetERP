namespace Persistence.entities.Stock;

public class Categorie:Common
{
    public string Nom { get; set; }
    public string Description { get; set; }
    public ICollection<Produit> Produits { get; set; }
}