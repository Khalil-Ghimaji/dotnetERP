
namespace Persistence.entities.Facturation;
using Commande;
public class Paiement
{
    public int Id { get; set; }
    public  DateTime Date { get; set; }
    public float Montant { get; set; }
    public MethodePaiement MethodePaiement { get; set; }
    public int FactureId { get; set; }
}