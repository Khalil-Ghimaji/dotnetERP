using Persistence.entities.Commande;
using Persistence.Repository.ClientRepositories;
using Persistence.Repository.CommandeRepositories;
using Persistence.Repository.StockRepositories.Contracts;

namespace GestionCommande.Services;

public class CommandeService : ICommandeService
{
    private readonly ICommandeRepo _commandeRepo;
    private readonly IArticleCommandeRepo _articleCommandeRepo;
    private readonly IProduitRepo _produitRepo;
    private readonly IArticleStockRepo _articleStockRepo;
    private readonly IClientRepo _clientRepo;

    public CommandeService(ICommandeRepo commandeRepo, IProduitRepo produitRepo,
        IArticleCommandeRepo articleCommandeRepo, IArticleStockRepo articleStockRepo, IClientRepo clientRepo)
    {
        _commandeRepo = commandeRepo;
        _produitRepo = produitRepo;
        _articleStockRepo = articleStockRepo;
        _clientRepo = clientRepo;
        _articleCommandeRepo = articleCommandeRepo;
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
        return await _commandeRepo.GetById(id);
    }

    public async Task<Commande> preparerCommande(Commande commande)
    {
        var client = await _clientRepo.GetById(commande.client.Id);
        if (client == null)
        {
            throw new HttpRequestException("Client inexistant");
        }

        commande.client = client;
        return await _commandeRepo.Add(commande);
    }

    public async Task<Commande> modifierCommande(Commande commande)
    {
        var command = await getCommandeById(commande.Id);
        if (command == null)
        {
            throw new HttpRequestException($"Commande n{commande.Id} n'existe pas");
        }

        if (command.status == StatusCommande.PREPARATION && commande.status == StatusCommande.PREPARATION)
        {
            _commandeRepo.Detach(command); // Detach the existing entity
            return await _commandeRepo.Update(commande);
        }

        throw new BadHttpRequestException("Commande n'est pas en préparation");
    }

    public async Task<Commande> ajouterArticle(int idCommande, int idProduit, int quantite)
    {
        var commande = await getCommandeById(idCommande);
        if (commande == null)
        {
            throw new HttpRequestException($"Commande n{idCommande} n'existe pas");
        }

        if (quantite < 1)
        {
            throw new BadHttpRequestException("Quantité doit être supérieure à 0");
        }
        if (commande.status == StatusCommande.PREPARATION)
        {
            var produit = await _produitRepo.GetById(idProduit);
            if (produit != null)
            {
                var articleStock = await _articleStockRepo.GetArticleStockByProduitId(idProduit);
                if (articleStock == null)
                {
                    throw new HttpRequestException($"Article {produit.Nom} n'est pas en stock");
                }
                var prix = articleStock.Prix;
                var articleCommande = _commandeRepo.getArticleCommandeByProduit(idCommande, idProduit);
                if (articleCommande == null)
                {
                    if(articleStock.Quantite<quantite)
                    {
                        throw new BadHttpRequestException($"Quantité de {produit.Nom} insuffisante");
                    }
                    var article = new ArticleCommande
                    {
                        commande = commande,
                        produit = produit,
                        quantite = quantite,
                        prix = prix
                    };
                    commande.articles.Add(article);
                }
                else
                {
                    articleCommande.quantite += quantite;
                    if(articleStock.Quantite<articleCommande.quantite)
                    {
                        throw new BadHttpRequestException($"Quantité de {produit.Nom} insuffisante");
                    }
                }

                return await _commandeRepo.Update(commande);
            }

            throw new HttpRequestException("Produit inexistant");
        }

        throw new BadHttpRequestException("Commande n'est pas en préparation");
    }

    public async Task<Commande> retirerArticle(int idCommande, int idProduit, int quantite)
    {
        var commande = await getCommandeById(idCommande);
        if (commande == null)
        {
            throw new HttpRequestException($"Commande n{idCommande} n'existe pas");
        }

        if (commande.status == StatusCommande.PREPARATION)
        {
            var articleCommande = _commandeRepo.getArticleCommandeByProduit(idCommande, idProduit);
            if (articleCommande != null)
            {
                if (articleCommande.quantite < quantite)
                {
                    throw new BadHttpRequestException("Quantité à retirer supérieure à la quantité commandée");
                }

                if (articleCommande.quantite == quantite)
                {
                    commande.articles.Remove(articleCommande);
                    _articleCommandeRepo.Delete(articleCommande.Id);
                }
                else
                {
                    articleCommande.quantite -= quantite;
                }

                return await _commandeRepo.Update(commande);
            }

            throw new HttpRequestException("Article inexistant");
        }

        throw new BadHttpRequestException("Commande n'est pas en préparation");
    }

    public async Task<Commande> validerCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande == null)
        {
            throw new HttpRequestException($"Commande n{id} n'existe pas");
        }

        if (commande.status == StatusCommande.PREPARATION)
        {
            commande.status = StatusCommande.VALIDEE;
            return await _commandeRepo.Update(commande);
        }

        throw new BadHttpRequestException("Commande n'est pas en préparation");
    }

    public async Task<Commande> annulerCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande == null)
        {
            throw new HttpRequestException($"Commande n{id} n'existe pas");
        }

        if (commande.status == StatusCommande.VALIDEE || commande.status == StatusCommande.PREPARATION ||
                commande.status == StatusCommande.RESERVEE || commande.status == StatusCommande.FACTUREE)
        {
            commande.status = StatusCommande.ANNULEE;
            return await _commandeRepo.Update(commande);
        }

        throw new BadHttpRequestException("Commande ne peut plus etre annulée");
    }

    public async Task<Commande> facturerCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande == null)
        {
            throw new HttpRequestException($"Commande n{id} n'existe pas");
        }

        if (commande.status == StatusCommande.VALIDEE)
        {
            commande.status = StatusCommande.FACTUREE;
            return await _commandeRepo.Update(commande);
        }

        throw new BadHttpRequestException($"Commande n{id} n'est pas validée");
    }
    
    public async Task<Commande> reserverCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande == null)
        {
            throw new HttpRequestException($"Commande n{id} n'existe pas");
        }

        if (commande.status == StatusCommande.FACTUREE)
        {
            commande.status = StatusCommande.RESERVEE;
            return await _commandeRepo.Update(commande);
        }

        throw new BadHttpRequestException("Commande n'est pas facturée");
    }

    public async Task<Commande> expedierCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande == null)
        {
            throw new HttpRequestException($"Commande n{id} n'existe pas");
        }

        if (commande.status == StatusCommande.FACTUREE || commande.status == StatusCommande.RESERVEE)
        {
            commande.status = StatusCommande.EXPEDIEE;
            return await _commandeRepo.Update(commande);
        }

        throw new BadHttpRequestException($"Commande n{id} n'est pas facturée");
    }

    // public async Task<Commande> livrerCommande(int id)
    // {
    //     var commande = await getCommandeById(id);
    //     if (commande == null)
    //     {
    //         throw new HttpRequestException($"Commande n{id} n'existe pas");
    //     }
    //
    //     if (commande.status == StatusCommande.EXPEDIEE)
    //     {
    //         commande.status = StatusCommande.LIVREE;
    //         return await _commandeRepo.Update(commande);
    //     }
    //
    //     throw new BadHttpRequestException($"Commande n{id} n'est pas expédiée");
    // }

    public async Task<Commande?> supprimerCommande(int id)
    {
        return await _commandeRepo.Delete(id);
    }

    public async Task<bool> commandeExists(int id)
    {
        var commande = await _commandeRepo.GetById(id);
        return commande != null;
    }
    
    public async Task<Commande> rollback(int id, StatusCommande lastStatus)
    {
        var commande = await getCommandeById(id);
        if (commande == null)
        {
            throw new HttpRequestException($"Commande n{id} n'existe pas");
        }

        commande.status = lastStatus;
        return await _commandeRepo.Update(commande);
    }

    public async Task<Commande> payerCommande(int id)
    {
        var commande = await getCommandeById(id);
        if (commande == null)
        {
            throw new HttpRequestException($"Commande n{id} n'existe pas");
        }
        if (commande.status != StatusCommande.FACTUREE && commande.status != StatusCommande.RESERVEE && commande.status != StatusCommande.EXPEDIEE)
        {
            throw new BadHttpRequestException("Commande n'est pas facturée");
        }
        commande.status = StatusCommande.PAYEE;
        return await _commandeRepo.Update(commande);
    }


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