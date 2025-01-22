using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.entities.Client;
using Persistence.entities.Commande;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionCommande.Repository;

public class CommandeRepo : GenericRepository<Commande>,ICommandeRepo
{
    public CommandeRepo(AppDbContext context) : base(context) {}
    public async Task<List<Commande>> getCommandesByClient(Client client)
    {
        return await _dbSet.Where(c => c.client == client).ToListAsync();
    }
    
    public ArticleCommande? getArticleCommandeByProduit(int idCommande, int idProduit)
    {
        return _context.ArticleCommandes.FirstOrDefault(ac => ac.commande.Id == idCommande && ac.produit.Id == idProduit);
    }
}