namespace Persistence.entities.Client;

public class Client:Common
{
    public int  Id { get; set; }
    public string Name { get; set; }
    public int telephone { get; set; }
    public string adresse { get; set; }
    public string email { get; set; }
    public bool estRestreint { get; set; }
    public int note { get; set; }
}
