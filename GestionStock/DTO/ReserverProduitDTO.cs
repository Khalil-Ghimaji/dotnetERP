namespace GestionStock.DTO;

public class ReserverProduitDTO
{
    public int ProduitId { get; init; }
    public int Quantite { get; init; }
    public TimeSpan ReservationDuration { get; init; }
}