using Persistence;
using Persistence.entities.Commande;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public class CommandeRepo : GenericRepository<Commande>,ICommandRepo
{
    public CommandeRepo(AppDbContext context) : base(context)
    {}
}