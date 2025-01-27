using Persistence.entities.Client;

namespace GestionClients.Services
{
    public interface IClientService
    {
        public  Task ajouterClient(Client client);
        public Task<Client?>  consulterClient(int id);
        public Task evaluerClientAsync(int id ,float note);
        public Task<IEnumerable<Client>> listerClients();
        public Task modifierClientAsync(Client client);
        public Task<List<Client>> FiltrerClients(Func<Client, bool> condition);
        public Task restaurerClient(int id);
        public Task restreindreClient(int id);
        public Task supprimerClient(int id);
    }
}
