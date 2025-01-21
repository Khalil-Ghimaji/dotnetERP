using Persistence;
using Persistence.entities.Client;
using Persistence.entities.Commande;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public class ClientsRepo : GenericRepository<Client>,IClientRepo
{
    public ClientsRepo(AppDbContext context) : base(context)
    {}
    public void ajouterClient(Client client)
    {
        _context.Clients.Add(client);
        _context.SaveChanges();
    }
    public Client? consulterClient(int id)
    {
        return _context.Clients.Find(id);
    }
    public void modifierClient(Client client)
    {
        var existingClient = _context.Clients.FirstOrDefault(c => c.Id == client.Id);
        if (existingClient != null)
        {
            existingClient.Name = client.Name;
            existingClient.telephone = client.telephone;
            existingClient.adresse = client.adresse;
            existingClient.email = client.email;
            existingClient.estRestreint = client.estRestreint;
            existingClient.note = client.note;
            _context.SaveChanges();
        }
    }
    public void supprimerClient(int id)
    {
        var client = _context.Clients.FirstOrDefault(c => c.Id == id);
        if (client != null)
        {
            _context.Clients.Remove(client);
            _context.SaveChanges();
        }
    }
}