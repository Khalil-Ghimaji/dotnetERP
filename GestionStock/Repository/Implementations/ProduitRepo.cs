using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public class ProduitRepo : GenericRepository<Produit>, IProduitRepo
{
    public ProduitRepo(AppDbContext context) : base(context)
    {
    }
    
    public async Task<bool> ProduitExists(int id,string nom)
    {
        return await _context.Produits.AnyAsync(p => p.Id == id || p.Nom == nom);
    }
}