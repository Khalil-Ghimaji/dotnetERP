using Persistence.entities.Commande;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public interface ICommandRepo:IGenericRepository<Commande>
{
    
}