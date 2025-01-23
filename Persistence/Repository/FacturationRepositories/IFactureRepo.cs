using Persistence.entities.Client;
using Persistence.entities.Facturation;
using Persistence.Repository;

namespace Persistence.Repository.FacturationRepositories;

public interface IFactureRepo : IGenericRepository<Facture>
{
}