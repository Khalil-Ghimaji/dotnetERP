namespace GestionStock.DTO;

public class AjouterProduitRequestDTO
{
    public string Nom { get; init; }
    public int CategoryId { get; init; }
    public int Quantite { get; init; }
    public double Prix { get; init; }
}

public class AjouterProduitResponseDTO
{
    public int Id { get; init; }
}