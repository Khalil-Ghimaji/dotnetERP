using Persistence.entities.Facturation;
using Persistence.Repository;

namespace Persistence.Repository.FacturationRepositories;

public interface IEcheanceRepo:IGenericRepository<Echeance>
{
    Task<IEnumerable<Echeance>> GetPaiementsByFactureId(int factureId);

}