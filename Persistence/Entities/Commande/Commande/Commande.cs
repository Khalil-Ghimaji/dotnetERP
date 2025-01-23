using Persistence.entities.Facturation;

namespace Persistence.entities.Commande;
using Client;

public record Commande
{
    public int Id { get; set; }
    public DateTime dateCommande { get; set; }
    public StatusCommande status { get; set; }
    public Client client { get; set; }
    public List<ArticleCommande> articles { get; set; }
    public Facture Facture { get; set; }
    
}