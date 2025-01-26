using Persistence.entities.Facturation;

namespace Persistence.DTO.Facturation
{
    public class UpdatePaiementDTO
    {
        public DateTime? Date { get; set; }
        public float? Montant { get; set; }
        public MethodePaiement? MethodePaiement { get; set; }
    }
}