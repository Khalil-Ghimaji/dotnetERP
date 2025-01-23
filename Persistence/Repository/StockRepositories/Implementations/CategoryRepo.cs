using Persistence.entities.Stock;
using Persistence.Repository.StockRepositories.Contracts;

namespace Persistence.Repository.StockRepositories.Implementations;

public class CategoryRepo : GenericRepository<Categorie>, ICategoryRepo
{
    public CategoryRepo(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> CategoryExists(int categoryId)
    {
        return await GetById(categoryId) != null;
    }
}