using Persistence.DTO.GestionClients;
using Persistence.entities.Client;

namespace GestionClients.Services
{
    public interface IClientService
    {
        public  Task ajouterClient(ClientIn client);
        public Task<ClientOut?>  consulterClient(int id);
        public Task evaluerClient(int id ,float note);
        public Task<IEnumerable<ClientOut>> listerClients();
        public Task modifierClient(ClientIn client, int id);
        public Task<List<ClientOut>> filtrerClients(Func<Client, bool> condition);
        public Task restaurerClient(int id);
        public Task restreindreClient(int id);
        public Task supprimerClient(int id);
    }
}
