namespace GestionStock.DTO;

public class ExpedierMarchandisesRequestDTO
{
    public int idCommande;
}
public class ArticleExpedierMarchandisesDTO
{
    public int ProduitId { get; init; }
    public int Quantite { get; init; }
}