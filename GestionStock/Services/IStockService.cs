using GestionStock.DTO;
using AjouterQuantiteRequestDTO = GestionStock.DTO.ArticleExpedierMarchandisesDTO;

namespace GestionStock.Services;

public interface IStockService
{
    Task<int> AjouterProduit(AjouterProduitRequestDTO dto);
    Task<IEnumerable<ArticleStockDTO>> ConsulterStock();
    Task ExpedierMarchandises(int idCommande);
    Task ModifierProduit(ProduitDTO dto);
    /*Task ReserverProduit(ReserverProduitRequestDTO dto);*/
    Task ReserverCommande(ReserverCommandeRequestDTO reserverCommande);
    Task ConfirmerCommande(int CommandeId);
    Task SupprimerProduit(int id);
    Task AjouterQuantite(AjouterQuantiteRequestDTO dto);
    Task<ArticleStockDTO> ConsulterProduit(int id);
    Task AnnulerCommande(int CommandeId);
}