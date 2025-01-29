using Persistence.entities.Facturation;
using Persistence.Repository;

namespace Persistence.Repository.FacturationRepositories;

public interface IPaiementRepo:IGenericRepository<Paiement>
{
    Task<IEnumerable<Paiement>> GetPaiementsByFactureId(int factureId);

}