using GestionStock.Repository;
using Microsoft.EntityFrameworkCore;
using Persistence.entities.Client;

namespace GestionClients.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepo _ClientRepo;
        public ClientService(IClientRepo stockRepo)
        {
            _ClientRepo = stockRepo;
        }
        public void ajouterClient(Client client)
        {
            _ClientRepo.ajouterClient(client);
            
        }
        public Client consulterClient(int id)
        {
        return _ClientRepo.consulterClient(id);
        }
        public void evaluerClient(int id, int note)
        {
            var client = _ClientRepo.consulterClient(id);
            if(client != null)
            {
                client.note = (client.note +note)/2 ;
                _ClientRepo.modifierClient(client);
            }
        }
        public void modifierClient(Client client)
        {
            var existingClient = _ClientRepo.consulterClient(client.Id);

            if (existingClient == null)
            {
                // Afficher un message d'erreur
                Console.WriteLine($"Erreur : Le client avec l'ID {client.Id} n'existe pas.");
                
            }
            else
            {
                _ClientRepo.modifierClient(client);
            }
        }
        public void restaurerClient(int id)
        {
            var client = _ClientRepo.consulterClient(id);
            client.estRestreint = false;
            _ClientRepo.modifierClient(client);
        }
        public void restreindreClient(int id)
        {
            var client = _ClientRepo.consulterClient(id);
            client.estRestreint = true;
            _ClientRepo.modifierClient(client);
        }
        public void supprimerClient(int id)
        {
            _ClientRepo.supprimerClient(id);
        }

    }
}
