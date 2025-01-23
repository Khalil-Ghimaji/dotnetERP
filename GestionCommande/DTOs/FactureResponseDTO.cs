namespace GestionCommande.DTOs;

public class FactureResponseDTO
{
    public int Id { get; set; }
    public DateTime DateGeneration { get; set; }
    public double MontantTotal { get; set; }
    public String StatusFacture { get; set; }
}