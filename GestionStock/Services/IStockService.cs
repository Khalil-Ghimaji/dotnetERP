using GestionStock.DTO;
using AjouterQuantiteRequestDTO = GestionStock.DTO.ArticleExpedierMarchandisesDTO;
namespace GestionStock.Services;

public interface IStockService
{
    Task<int> AjouterProduit(AjouterProduitRequestDTO dto);
    Task<IEnumerable<ArticleStockDTO>> ConsulterStock();
    Task ExpedierMarchandises(ExpedierMarchandisesRequestDTO commande);
    Task ModifierProduit(ProduitDTO dto);
    Task<Guid> ReserverProduit(ReserverProduitRequestDTO dto);
    void ConfirmerCommande(Guid id);
    Task SupprimerProduit(int id);
    Task AjouterQuantite(AjouterQuantiteRequestDTO dto);
    Task<ArticleStockDTO> ConsulterProduit(int id);
}