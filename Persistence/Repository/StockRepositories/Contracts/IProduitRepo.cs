using Persistence.entities.Stock;

namespace Persistence.Repository.StockRepositories.Contracts;

public interface IProduitRepo : IGenericRepository<Produit>
{
    //chercher un produit par son nom ou son id
    Task<bool> ProduitExists(int id, string nom);
}