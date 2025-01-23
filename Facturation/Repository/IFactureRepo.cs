using Persistence.entities.Client;
using Persistence.entities.Facturation;
using Persistence.Repository;

namespace Facturation.Repository;

public interface IFactureRepo : IGenericRepository<Facture>
{
}