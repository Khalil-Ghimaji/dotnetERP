using Persistence.entities.Client;
using Persistence.entities.Commande;
using Persistence.Repository;

namespace Persistence.Repository.CommandeRepositories;

public interface ICommandeRepo:IGenericRepository<Commande>
{
    public Task<List<Commande>> getCommandesByClient(Client client);
    public ArticleCommande? getArticleCommandeByProduit(int idCommande, int idProduit);
    // public Task<Commande?> getEagerById(int id);
    void Detach(Commande entity);
    
}