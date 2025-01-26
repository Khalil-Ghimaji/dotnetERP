using System.Text.Json.Serialization;

namespace Persistence.entities.Facturation
{
    using Persistence.entities.Commande;

    public class Facture 
    {
        public int FactureId { get; set; }
        public int CommandeId { get; set; }
        
        [JsonIgnore]
        public virtual Commande Commande { get; set; } 
        public DateTime DateGeneration { get; set; }
        public float MontantTotal { get; set; }

        public StatusFacture StatusFacture { get; set; }
        public float MontantPayé { get; set; }

    }
}