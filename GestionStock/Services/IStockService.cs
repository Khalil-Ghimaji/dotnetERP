using Persistence.entities.Commande;
using Persistence.entities.Stock;

namespace GestionStock.Services;

public interface IStockService
{
    void AjouterProduit(int produitId, int quantite);
    Task<IEnumerable<ArticleStock>> ConsulterStock();
    Task ExpedierMarchandises(Commande commande);
    Task ModifierProduit(int id, int quantite);
    Task ReserverProduit(int id, int quantite, TimeSpan reservationDuration);
    void ConfirmerCommande(int id);
    Task SupprimerProduit(int id);
}