using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.entities.Client;
using Persistence.entities.Commande;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace Persistence.Repository.CommandeRepositories;

public class CommandeRepo : GenericRepository<Commande>,ICommandeRepo
{
    public CommandeRepo(AppDbContext context) : base(context) {}
    public async Task<IEnumerable<Commande>> GetAll()
    {
        return await _dbSet
            .Include(c=>c.client)
            .Include(c=>c.articles)
            .Include(c=>c.Facture)
            .ToListAsync();
    }

    // public async Task<Commande?> getEagerById(int id)
    // {
    //     return await _dbSet
    //         .Include(c=>c.client)
    //         .Include(c=>c.articles)
    //         .Include(c=>c.Facture)
    //         .FirstOrDefaultAsync(c => c.Id == id);
    // }
    public async Task<List<Commande>> getCommandesByClient(Client client)
    {
        return await _dbSet.Where(c => c.client == client)
            .Include(c=>c.client)
            .Include(c=>c.articles)
            .Include(c=>c.Facture)
            .ToListAsync();
    }
    
    public ArticleCommande? getArticleCommandeByProduit(int idCommande, int idProduit)
    {
        return _context.ArticleCommandes.FirstOrDefault(ac => ac.commande.Id == idCommande && ac.produit.Id == idProduit);
    }
    
    public void Detach(Commande entity)
    {
        _context.Entry(entity).State = EntityState.Detached;
    }
}