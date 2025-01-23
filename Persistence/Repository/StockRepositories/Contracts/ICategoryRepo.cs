using Persistence.entities.Stock;

namespace Persistence.Repository.StockRepositories.Contracts;

public interface ICategoryRepo:IGenericRepository<Categorie>
{
    Task<bool> CategoryExists(int categoryId);
}