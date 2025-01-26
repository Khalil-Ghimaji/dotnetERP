using Persistence.entities.Facturation;

namespace Persistence.DTO.Facturation
{
    public class UpdateFactureDTO
    {
        public float? MontantTotal { get; set; }
        public StatusFacture? StatusFacture { get; set; }
        public float? MontantPayé { get; set; }

    }
}