namespace GestionStock.DTO;

public record CreerProduitDTO(
    int Id,
    string Nom,
    double Prix,
    int CategorieId,
    int Quantite
);
