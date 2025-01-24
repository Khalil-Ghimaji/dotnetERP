using Persistence.entities.Facturation;

namespace Persistence.entities.Commande;
using Client;

public record Commande
{
    public int Id { get; set; }
    public DateTime dateCommande { get; set; }
    public StatusCommande status { get; set; }
    public virtual Client client { get; set; }
    public virtual List<ArticleCommande> articles { get; set; }
    public virtual Facture Facture { get; set; }
    
}