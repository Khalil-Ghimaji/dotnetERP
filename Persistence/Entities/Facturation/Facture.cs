namespace Persistence.entities.Facturation
{
    using Persistence.entities.Commande;

    public class Facture 
    {
        public int FactureId { get; set; }
        public int CommandeId { get; set; }
        public Commande Commande { get; set; } 
        public DateTime DateGeneration { get; set; }
        public float MontantTotal { get; set; }
        public StatusFacture StatusFacture { get; set; }
    }
}
