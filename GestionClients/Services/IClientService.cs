using Persistence.entities.Client;

namespace GestionClients.Services
{
    public interface IClientService
    {
        public void ajouterClient(Client client);
        public Client consulterClient(int id);
        public void evaluerClient(int id ,int note);
        public void modifierClient(Client client);
        public void restaurerClient(int id);
        public void restreindreClient(int id);
        public void supprimerClient(int id);
    }
}
