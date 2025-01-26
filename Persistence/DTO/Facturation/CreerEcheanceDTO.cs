using Persistence.entities.Facturation;

namespace Persistence.DTO.Facturation
{
    public class CreerEcheanceDTO
    {
        public int FactureId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public float Montant { get; set; }
        public MethodePaiement MethodePaiement { get; set; }
        public StatutEcheance StatutEcheance { get; set; } = StatutEcheance.Impayée;


    }
}