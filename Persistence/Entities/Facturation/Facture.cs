namespace Persistence.entities.Facturation;
using Commande;
public class Facture
{
    public int FactureId { get; set; }
    public int CommandeId { get; set; }
    public Commande commande { get; set; } 
    public DateTime DateGeneration { get; set; }
    public double MontantTotal { get; set; }
    public StatusFacture StatusFacture { get; set; }
}