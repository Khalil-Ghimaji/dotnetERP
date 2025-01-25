namespace GestionStock.DTO;

public class ExpedierMarchandisesRequestDTO
{
    public IEnumerable<ArticleExpedierMarchandisesDTO> Articles { get; init; }
}
public class ArticleExpedierMarchandisesDTO
{
    public int ProduitId { get; init; }
    public int Quantite { get; init; }
}