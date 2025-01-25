namespace GestionStock.DTO;

public class ProduitDTO
{
    public int ProduitId { get; init; }
    public string Nom { get; init; }
    public int CategoryId { get; init; }
    public int Quantite { get; init; }
    public double Prix { get; init; }
}