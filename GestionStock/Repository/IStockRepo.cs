using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public interface IStockRepo:IGenericRepository<ArticleStock>
{
    
}