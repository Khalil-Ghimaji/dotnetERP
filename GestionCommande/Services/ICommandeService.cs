using Persistence.entities.Commande;

namespace GestionCommande.Services;

public interface ICommandeService
{
    Task<IEnumerable<Commande>> getAllCommandes();
    Task<Commande?> getCommandeById(int id);
    Task<Commande> preparerCommande(Commande commande);
    Task<Commande> modifierCommande(int id, int idClient, DateTime? dateCommande);
    Task<Commande?> supprimerCommande(int id);
    Task<Commande> ajouterArticle(int idCommande, int idProduit, int quantite);
    Task<Commande> retirerArticle(int idCommande, int idProduit, int quantite);
    Task<Commande> validerCommande(int id);
    Task<Commande> annulerCommande(int id);
    Task<Commande> reserverCommande(int id);
    Task<Commande> expedierCommande(int id);
    Task<Commande> payerCommande(int id);
    Task<Commande> facturerCommande(int id);
    Task<bool> commandeExists(int id);
    Task<Commande> rollback(int id, StatusCommande lastStatus);
    //Task<Commande> livrerCommande(int id);
}