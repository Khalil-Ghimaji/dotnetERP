

namespace Facturation.DTO
{
    public class UpdatePaiementDTO
    {
        public DateTime? Date { get; set; }
        public float? Montant { get; set; }
        public String? MethodePaiement { get; set; }
    }
}