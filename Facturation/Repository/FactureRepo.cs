using Persistence;
using Persistence.entities.Client;
using Persistence.entities.Commande;
using Persistence.entities.Facturation;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public class FacturesRepo : GenericRepository<Facture>,IFactureRepo
{
    public FacturesRepo(AppDbContext context) : base(context)
    {}
}