namespace AppGateway.DTOs.GestionFactureDTOs
{
    public class CreerPaiementDTO
    {
        public int FactureId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public float Montant { get; set; }
        public String MethodePaiement { get; set; }
        
    }
}