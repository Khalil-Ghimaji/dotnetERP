using Microsoft.EntityFrameworkCore;
using Persistence.entities.Stock;
using Persistence.Repository.StockRepositories.Contracts;

namespace Persistence.Repository.StockRepositories.Implementations;

public class ProduitRepo : GenericRepository<Produit>, IProduitRepo
{
    public ProduitRepo(AppDbContext context) : base(context)
    {
    }
    
    public async Task<bool> ProduitExists(string nom)
    {
        return await _context.Produits.AnyAsync(p => p.Nom == nom);
    }
}