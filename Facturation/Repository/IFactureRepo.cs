using Persistence.entities.Client;
using Persistence.entities.Facturation;
using Persistence.Repository;

namespace GestionStock.Repository;

public interface IFactureRepo : IGenericRepository<Facture>
{
}