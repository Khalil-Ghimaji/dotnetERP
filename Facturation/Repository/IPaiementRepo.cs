using Persistence.entities.Facturation;
using Persistence.Repository;

namespace Facturation.Repository;

public interface IPaiementRepo:IGenericRepository<Paiement>
{
    Task<IEnumerable<Paiement>> GetPaiementsByFactureId(int factureId);

}