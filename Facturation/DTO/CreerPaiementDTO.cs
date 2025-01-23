using Persistence.entities.Facturation;

namespace Facturation.DTO
{
    public class CreerPaiementDTO
    {
        public int FactureId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public float Montant { get; set; }
        public MethodePaiement MethodePaiement { get; set; }
        
    }
}