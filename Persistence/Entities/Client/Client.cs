using System.ComponentModel.DataAnnotations;

namespace Persistence.entities.Client;

public class Client 
{
    public int Id { get; set; }
    [Required]
    public string nom { get; set; }
    public string address { get; set; }
    public int telephone { get; set; }
    public float sumNotes { get; set; }
    public int nbNotes { get; set; }
    public float note { get; set; }
    [Required]
    public bool estRestreint { get; set; } = false;

}