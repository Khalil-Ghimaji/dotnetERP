using Persistence.entities.Client;
using Persistence.entities.Facturation;
using Persistence.Repository;

namespace GestionStock.Repository;

public interface IClientRepo : IGenericRepository<Client> 
{
    public void ajouterClient(Client client);
    public Client consulterClient(int id);
    public void modifierClient(Client client);
    public void supprimerClient(int id);
}