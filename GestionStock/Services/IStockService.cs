using GestionStock.DTO;
using Persistence.entities.Commande;
using Persistence.entities.Stock;

namespace GestionStock.Services;

public interface IStockService
{
    Task AjouterProduit(ProduitDTO dto);
    Task<IEnumerable<ArticleStockDTO>> ConsulterStock();
    Task ExpedierMarchandises(Commande commande);
    Task ModifierProduit(ProduitDTO dto);
    Task<Guid> ReserverProduit(ReserverProduitDTO dto);
    void ConfirmerCommande(Guid id);
    Task SupprimerProduit(int id);
    void AjouterQuantite(ArticleStockDTO dto);
    Task<ArticleStockDTO> ConsulterProduit(int id);
}