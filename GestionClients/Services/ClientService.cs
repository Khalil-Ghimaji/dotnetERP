using Microsoft.EntityFrameworkCore;
using Persistence.entities.Client;
using Persistence.Repository.ClientRepositories;

namespace GestionClients.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepo _ClientRepo;
        public ClientService(IClientRepo clientRepo)
        {
            _ClientRepo = clientRepo;
        }
        public async Task  ajouterClient(Client client)
        {
            await _ClientRepo.Add(client);
            
        }
        public Task<Client?> consulterClient(int id)
        {
        return _ClientRepo.GetById(id);
        }
        public async Task<IEnumerable<Client>> listerClients()
        {
            return await _ClientRepo.GetAll();
        }
        public async Task evaluerClientAsync(int id, float note)
        {
            var client = await _ClientRepo.GetById(id);
            if (client == null)
            {
                throw new KeyNotFoundException($"Client avec l'ID {id} introuvable.");
            }
            client.sumNotes = client.sumNotes +note  ;
            client.nbNotes++;
            client.note = client.sumNotes / client.nbNotes;
            await _ClientRepo.Update(client);
            
            
        }
        public async Task modifierClientAsync(Client client)
        {
            var existingClient = await _ClientRepo.GetById(client.Id);
            if (existingClient == null)
            {
                throw new KeyNotFoundException($"Client avec l'ID {client.Id} introuvable.");
            }

            
                _ClientRepo.Update(client);
            
        }
        public async Task<List<Client>> FiltrerClients(Func<Client, bool> condition)
        {
            // Récupère tous les clients depuis le dépôt
            var clients = await _ClientRepo.GetAll();

            // Filtre les clients en utilisant la condition
            var filteredClients = clients.Where(condition).ToList();

            return filteredClients;
        }
        public async Task restaurerClient(int id)
        {
            var client = await _ClientRepo.GetById(id);
            if (client == null)
            {
                throw new KeyNotFoundException($"Client avec l'ID {id} introuvable.");
            }
            client.estRestreint = false;
            await _ClientRepo.Update(client);
        }

        public async Task restreindreClient(int id)
        {
            var client = await _ClientRepo.GetById(id);
            if (client == null)
            {
                // Lever une exception si le client n'existe pas
                throw new KeyNotFoundException($"Client avec l'ID {id} introuvable.");
            }
            client.estRestreint = true;
            await _ClientRepo.Update(client);
        }
        public async Task supprimerClient(int id)
        {
           await _ClientRepo.Delete(id);
        }

    }
}
