using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.entities.Client;
using Persistence.entities.Commande;
using Persistence.entities.Facturation;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace Persistence.Repository.FacturationRepositories;

public class EcheanceRepo : GenericRepository<Echeance>,IEcheanceRepo
{
    public EcheanceRepo(AppDbContext context) : base(context)
    {}


    public async Task<IEnumerable<Echeance>> GetPaiementsByFactureId(int factureId)
    {
        return await _context.Paiements
            .Where(p => p.FactureId == factureId)
            .ToListAsync();
    }
}