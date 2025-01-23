using Persistence;
using Persistence.entities.Client;
using Persistence.entities.Commande;
using Persistence.entities.Facturation;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace Persistence.Repository.FacturationRepositories;

public class FactureRepo : GenericRepository<Facture>,IFactureRepo
{
    public FactureRepo(AppDbContext context) : base(context)
    {}
}