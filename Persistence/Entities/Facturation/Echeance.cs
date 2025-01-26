namespace Persistence.entities.Facturation;

public class Echeance
{
    
    public int PaiementId { get; set; }
    public  DateTime Date { get; set; }
    public float Montant { get; set; }
    public MethodePaiement MethodePaiement { get; set; }
    public int FactureId { get; set; }
    public StatutEcheance StatutEcheance  { get; set; }

}
