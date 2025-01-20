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
}