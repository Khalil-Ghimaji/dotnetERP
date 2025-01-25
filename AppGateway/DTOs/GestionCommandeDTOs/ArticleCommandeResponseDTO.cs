namespace GestionCommande.DTOs;

public class ArticleCommandeResponseDTO
{
    public int Id { get; set; }
    public ProduitResponseDTO produit { get; set; }
    public int quantite { get; set; }
    public double prix { get; set; }
}