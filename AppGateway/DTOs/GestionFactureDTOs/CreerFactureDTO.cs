namespace AppGateway.DTOs.GestionFactureDTOs
{
    public class CreerFactureDTO
    {
        public int CommandeId { get; set; }
        public DateTime DateGeneration { get; set; } = DateTime.Now;
        public float MontantTotal { get; set; }
        public String StatusFacture { get; set; } = "Créée";
    }
}