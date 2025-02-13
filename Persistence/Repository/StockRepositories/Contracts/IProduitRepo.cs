using Persistence.entities.Stock;

namespace Persistence.Repository.StockRepositories.Contracts;

public interface IProduitRepo : IGenericRepository<Produit>
{

    Task<bool> ProduitExists(string nom);
}