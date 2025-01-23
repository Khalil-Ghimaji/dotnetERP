using Persistence.entities.Facturation;

namespace Facturation.DTO
{
    public class UpdateFactureDTO
    {
        public float? MontantTotal { get; set; }
        public StatusFacture? StatusFacture { get; set; }
    }
}