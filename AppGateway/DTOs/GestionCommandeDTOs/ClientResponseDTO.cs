namespace GestionCommande.DTOs;

public class ClientResponseDTO
{
    public int Id { get; set; }
    public string nom { get; set; }
    public string address { get; set; }
    public int telephone { get; set; }
    public float note { get; set; }
    public bool estRestreint { get; set; }
}