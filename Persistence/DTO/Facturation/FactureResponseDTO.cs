using Persistence.entities.Facturation;

namespace Persistence.DTO.Facturation
{
    public class FactureResponseDTO
    {
        public int FactureId { get; set; }
        public int CommandeId { get; set; }
        public DateTime DateGeneration { get; set; }
        public float MontantTotal { get; set; }
        public String StatusFacture { get; set; }
        public float MontantPayé { get; set; }
    }
}
