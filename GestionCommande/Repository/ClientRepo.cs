using Persistence;
using Persistence.entities.Client;
using Persistence.Repository;

namespace GestionCommande.Repository;

public class ClientRepo : GenericRepository<Client>, IClientRepo
{
    public ClientRepo(AppDbContext context) : base(context)
    { }
}