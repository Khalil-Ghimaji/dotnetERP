using Persistence.entities.Facturation;

namespace Persistence.DTO.Facturation
{
    public class UpdateEcheanceDTO
    {
        public float? MontantTotal { get; set; }
        public StatusFacture? StatusFacture { get; set; }
    }
}