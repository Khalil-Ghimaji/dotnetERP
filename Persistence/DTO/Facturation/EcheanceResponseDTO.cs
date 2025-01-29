using Persistence.entities.Facturation;

namespace Persistence.DTO.Facturation
{
    public class EcheanceResponseDTO
    {
        public int EcheanceId { get; set; }
        public DateTime Date { get; set; }
        public float Montant { get; set; }
        public MethodePaiement MethodePaiement { get; set; }
        public StatutEcheance StatutEcheance { get; set; }
        public int FactureId { get; set; }
    }
}