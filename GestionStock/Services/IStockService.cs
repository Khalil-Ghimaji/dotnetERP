using GestionStock.DTO;
using Persistence.entities.Commande;
using Persistence.entities.Stock;

namespace GestionStock.Services;

public interface IStockService
{
    Task AjouterProduit(CreerProduitDTO dto);
    Task CreerProduit(CreerProduitDTO dto);
    Task<IEnumerable<ArticleStockDTO>> ConsulterStock();
    Task ExpedierMarchandises(Commande commande);
    Task ModifierProduit(ModifierProduitDTO dto);
    Task ReserverProduit(ReserverProduitDTO dto);
    void ConfirmerCommande(int id);
    Task SupprimerProduit(int id);
}