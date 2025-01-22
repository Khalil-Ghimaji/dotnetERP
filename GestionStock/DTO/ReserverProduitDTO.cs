namespace GestionStock.DTO;

public record ReserverProduitDTO(
    int ProduitId,
    int Quantite,
    TimeSpan ReservationDuration
    );

public record ModifierProduitDTO(
    int ProduitId,
    int Quantite,
    int CategoryId,
    string Nom
);
    