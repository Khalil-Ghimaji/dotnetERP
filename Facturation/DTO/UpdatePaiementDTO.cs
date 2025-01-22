using Persistence.entities.Facturation;

namespace Facturation.DTO
{
    public class UpdatePaiementDTO
    {
        public DateTime? Date { get; set; }
        public float? Montant { get; set; }
        public MethodePaiement? MethodePaiement { get; set; }
    }
}