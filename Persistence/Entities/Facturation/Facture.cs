namespace Persistence.entities.Facturation;

public class Facture
{
    public int FactureId { get; set; }
    public int CommandeId { get; set; }
    public Commande Commande { get; set; } 
    public DateTime DateGeneration { get; set; }
    public float MontantTotal { get; set; }
    public StatusFacture StatusFacture { get; set; }
}