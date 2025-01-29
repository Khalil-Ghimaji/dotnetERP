using Persistence.entities.Facturation;

namespace Persistence.DTO.Facturation
{
    public class CreerFactureDTO
    {
        public int CommandeId { get; set; }
        public DateTime DateGeneration { get; set; } = DateTime.Now;
        
        public float? MontantTotal { get; set; }
        public StatusFacture StatusFacture { get; set; } = StatusFacture.Créée;
        public float MontantPayé { get; set; } = 0;

    }
}