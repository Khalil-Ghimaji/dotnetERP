using Microsoft.EntityFrameworkCore;
using Persistence.entities.Client;
using Persistence.Repository.ClientRepositories;
using Persistence.DTO.GestionClients;

namespace GestionClients.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepo _ClientRepo;
        public ClientService(IClientRepo clientRepo)
        {
            _ClientRepo = clientRepo;
        }
        public async Task  ajouterClient(ClientIn dto)
        {
            var client = new Client
            {
                nom = dto.nom,
                address = dto.address,
                telephone = dto.telephone,
                sumNotes = 0,
                nbNotes = 0,
                note = 0,
                estRestreint = false
            };
            await _ClientRepo.Add(client);
            
        }
        
        public async Task<ClientOut?> consulterClient(int id)
        {
            var client = await _ClientRepo.GetById(id);
            if (client == null)
            {
                return null;
            }
            var clientOut = new ClientOut
            {
                Id = client.Id,
                nom = client.nom,
                address = client.address,
                telephone = client.telephone,
                note = client.note,
                estRestreint = client.estRestreint
            };

            return clientOut;
        }

        public async Task<IEnumerable<ClientOut>> listerClients()
        {
            var clients = await _ClientRepo.GetAll();
            if( clients == null)
            {
                return null;
            }
            var clientsOut = clients.Select(client => new ClientOut
            {
                Id = client.Id,
                nom = client.nom,
                address = client.address,
                telephone = client.telephone,
                note = client.note,
                estRestreint = client.estRestreint
            });

            return clientsOut;
        }

        public async Task evaluerClient(int id, float note)
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
        public async Task modifierClient(ClientIn client, int id)
        {
            var existingClient = await _ClientRepo.GetById(id);
            if (existingClient == null)
            {
                throw new KeyNotFoundException($"Client avec l'ID {id} introuvable.");
            }

            existingClient.nom = client.nom;
            existingClient.address = client.address;
            existingClient.telephone = client.telephone;
            await _ClientRepo.Update(existingClient);
        }

        public async Task<List<ClientOut>> filtrerClients(Func<Client, bool> condition)
        {
            var clients = await _ClientRepo.GetAll();
            var filteredClients = clients.Where(condition).ToList();
            var filteredClientsOut = filteredClients.Select(client => new ClientOut
            {
                Id = client.Id,
                nom = client.nom,
                address = client.address,
                telephone = client.telephone,
                note = client.note,
                estRestreint = client.estRestreint
            }).ToList();

            return filteredClientsOut;
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
