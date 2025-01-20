using Persistence;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public class StockRepo : GenericRepository<ArticleStock>,IStockRepo
{
    public StockRepo(AppDbContext context) : base(context)
    {}
}