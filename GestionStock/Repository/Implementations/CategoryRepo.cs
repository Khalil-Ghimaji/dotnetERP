using Persistence;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

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