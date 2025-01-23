namespace GestionStock.DTO;

public class ArticleStockDTO
{
    public int Quantite { get; init; }
    public int ProduitId { get; init; }
    public string ProduitNom { get; init; }
    public int CategorieId { get; init; }
    public string CategorieNom { get; init; }
    public string CategoryDescription { get; init; }
    public double Prix { get; init; }
}