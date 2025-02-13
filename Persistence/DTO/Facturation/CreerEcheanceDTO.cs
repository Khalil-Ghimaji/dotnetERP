using Persistence.entities.Facturation;

namespace Persistence.DTO.Facturation
{
    public class CreerEcheanceDTO
    {
        public float Montant { get; set; }
        public MethodePaiement MethodePaiement { get; set; }
    }
}
