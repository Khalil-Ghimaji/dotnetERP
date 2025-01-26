namespace GestionStock.DTO;

public class ReserverProduitRequestDTO : ArticleExpedierMarchandisesDTO
{
    public string ReservationDuration { get; init; }
    public int CommandeId { get; init; }
}