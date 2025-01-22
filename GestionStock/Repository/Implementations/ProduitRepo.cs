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

    public async Task<bool> ProduitExists(string nom)
    {
        return await _context.Produits.AnyAsync(p => p.Nom == nom);
    }

    public async Task<bool> ProduitExists(int id)
    {
        return await GetById(id) != null;
    }
}