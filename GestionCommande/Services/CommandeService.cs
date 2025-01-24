using Persistence.entities.Commande;
using Persistence.Repository.ClientRepositories;
using Persistence.Repository.CommandeRepositories;
using Persistence.Repository.StockRepositories.Contracts;

namespace GestionCommande.Services;

public class CommandeService : ICommandeService
{
    private readonly ICommandeRepo _commandeRepo;
    private readonly IProduitRepo _produitRepo;
    private readonly IArticleStockRepo _articleStockRepo;
    private readonly IClientRepo _clientRepo;
    public CommandeService(ICommandeRepo commandeRepo, IProduitRepo produitRepo, IArticleStockRepo articleStockRepo, IClientRepo clientRepo)
    {
        _commandeRepo = commandeRepo;
        _produitRepo = produitRepo;
        _articleStockRepo = articleStockRepo;
        _clientRepo = clientRepo;
    }
    
    public async Task<IEnumerable<Commande>> getAllCommandes()
    {
        return await _commandeRepo.GetAll();
    }
    
    // public async Task<List<Commande>> getCommandesByClient(Client client)
    // {
    //     return await _repo.getCommandesByClient(client);
    // }
    
    public async Task<Commande?> getCommandeById(int id)
    {
        return await _commandeRepo.getEagerById(id);
    }

    public async Task<Commande?> preparerCommande(Commande commande)
    {
        var client = await _clientRepo.GetById(commande.client.Id);
        if (client == null)
        {
            return null;
        }
        commande.client = client;
        return await _commandeRepo.Add(commande);
    }
    
    public async Task<Commande?> modifierCommande(Commande commande)
    {
        var command = await getCommandeById(commande.Id);
        if (command != null && command.status == StatusCommande.PREPARATION && commande.status==StatusCommande.PREPARATION)
        {
            _commandeRepo.Detach(command); // Detach the existing entity
            return await _commandeRepo.Update(commande);
        }
        return null;
    }
    
    public async Task<Commande?> ajouterArticle(int idCommande, int idProduit, int quantite)
    {
        var commande = await getCommandeById(idCommande);
        if (commande != null && commande.status == StatusCommande.PREPARATION)
        {
            var produit = await _produitRepo.GetById(idProduit);
            if (produit != null)
            {
                var articleStock = await _articleStockRepo.GetArticleStockByProduitId(idProduit);
                if (articleStock == null)
                {
                    return null;
                }
                var prix = articleStock.Prix;
                var articleCommande = _commandeRepo.getArticleCommandeByProduit(idCommande, idProduit);
                if (articleCommande == null)
                {
                    var article = new ArticleCommande
                    {
                        commande = commande,
                        produit = produit,
                        quantite = quantite,
                        prix = prix
                    };
                    commande.articles.Add(article);
                }
                else{
                    articleCommande.quantite += quantite;
                }
                return await _commandeRepo.Update(commande);
            }
        }
        return null;
    }
    public async Task<Commande?> retirerArticle(int idCommande, int idProduit, int quantite)
    {
        var commande = await getCommandeById(idCommande);
        if (commande != null && commande.status == StatusCommande.PREPARATION)
        {
            var articleCommande = _commandeRepo.getArticleCommandeByProduit(idCommande, idProduit);
            if (articleCommande != null)
            {
                if (articleCommande.quantite < quantite)
                {
                    return null;
                }
                if(articleCommande.quantite == quantite)
                {
                    commande.articles.Remove(articleCommande);
                }
                else
                {
                    articleCommande.quantite -= quantite;
                }
                return await _commandeRepo.Update(commande);
            }
        }
        return null;
    }
    
    public async Task<Commande?> validerCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande !=null && commande.status == StatusCommande.PREPARATION)
        {
            commande.status = StatusCommande.VALIDEE;
            return await _commandeRepo.Update(commande);
        }
        return null;
    }

    public async Task<Commande?> annulerCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande != null && (commande.status == StatusCommande.VALIDEE || commande.status == StatusCommande.PREPARATION))
        {
            commande.status = StatusCommande.ANNULEE;
            return await _commandeRepo.Update(commande);
        }
        return null;
    }
    
    public async Task<Commande?> facturerCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande != null && commande.status == StatusCommande.VALIDEE)
        {
            commande.status = StatusCommande.FACTUREE;
            return await _commandeRepo.Update(commande);
        }
        return null;
    }

    public async Task<Commande?> expedierCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande != null && commande.status == StatusCommande.FACTUREE)
        {
            commande.status = StatusCommande.EXPEDIEE;
            return await _commandeRepo.Update(commande);
        }
        return null;
    }

    public async Task<Commande?> livrerCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande != null && commande.status == StatusCommande.EXPEDIEE)
        {
            commande.status = StatusCommande.LIVREE;
            return await _commandeRepo.Update(commande);
        }
        return null;
    }
    
    public async Task<Commande?> supprimerCommande(int id)
    {
        return await _commandeRepo.Delete(id);
    }

    public async Task<bool> commandeExists(int id)
    {
        var commande = await _commandeRepo.GetById(id);
        return commande != null;
    }

    // public async void payerCommande(Commande commande)
    // {
    //     commande.status = StatusCommande.PAYEE;
    //     await _repo.Update(commande);
    // }
    
    
    // public async void rembourserCommande(Commande commande)
    // {
    //     commande.status = StatusCommande.REMBOURSEE;
    //     await _repo.Update(commande);
    // }
    
    // public async void retournerCommande(Commande commande)
    // {
    //     commande.status = StatusCommande.RETOURNEE;
    //     await _repo.Update(commande);
    // }
    
    // public async void modifierStatusCommande(Commande commande, StatusCommande status)
    // {
    //     commande.status = status;
    //     await _repo.Update(commande);
    // }
}