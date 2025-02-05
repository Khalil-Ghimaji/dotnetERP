using Persistence.entities.Stock;

namespace GestionCommande.DTOs;

public class ArticleCommandeResponseDTO
{


    public int IdProduit { get; set; }
    public string Nom { get; set; }
    public String Categorie { get; set; }
    public int quantite { get; set; }
    public double prix { get; set; }
}