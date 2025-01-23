using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public interface ICategoryRepo:IGenericRepository<Categorie>
{
    Task<bool> CategoryExists(int categoryId);
}