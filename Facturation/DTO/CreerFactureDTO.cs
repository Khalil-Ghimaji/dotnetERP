using System;
using Persistence.entities.Facturation;

namespace Facturation.DTO
{
    public class CreerFactureDTO
    {
        public int CommandeId { get; set; }
        public DateTime DateGeneration { get; set; } = DateTime.Now;
        public float MontantTotal { get; set; }
        public StatusFacture StatusFacture { get; set; } = StatusFacture.Créée;
    }
}