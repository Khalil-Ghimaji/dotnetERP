using Persistence;
using Persistence.entities.Client;
using Persistence.entities.Commande;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace Persistence.Repository.ClientRepositories;

public class ClientRepo : GenericRepository<Client>,IClientRepo
{
    public ClientRepo(AppDbContext context) : base(context)
    {}
}