using Persistence.entities.Commande;
using Persistence.entities.Stock;

namespace GestionStock.Services;

public interface IStockService
{
    void AjouterProduit(Guid produitId, int quantite);
    Task<IEnumerable<ArticleStock>> ConsulterStock();
    Task ExpedierMarchandises(Commande commande);
    Task ModifierProduit(Guid id, int quantite);
    Task ReserverProduit(Guid id, int quantite, TimeSpan reservationDuration);
    void ConfirmerCommande(Guid id);
    Task SupprimerProduit(Guid id);
}