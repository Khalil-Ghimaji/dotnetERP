namespace GestionStock.DTO;

public class ReserverProduitRequestDTO : ArticleExpedierMarchandisesDTO
{
    public string ReservationDuration { get; init; }
}

public class ReserverProduitResponseDTO
{
    public string ReservationId { get; init; }
}