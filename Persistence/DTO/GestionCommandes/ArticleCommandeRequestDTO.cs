namespace GestionCommande.DTOs;

public record ArticleCommandeRequestDTO()
{

    public int IdProduit { get; set; }
    public int Quantite { get; set; }
}