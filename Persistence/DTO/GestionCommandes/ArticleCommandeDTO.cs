namespace GestionCommande.DTOs;

public record ArticleCommandeDTO()
{
    public int IdCommande { get; set; }
    public int IdProduit { get; set; }
    public int Quantite { get; set; }
}