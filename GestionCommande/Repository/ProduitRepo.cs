using Persistence;
using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionCommande.Repository;

public class ProduitRepo : GenericRepository<Produit>, IProduitRepo
{
    public ProduitRepo(AppDbContext context) : base(context) {}
}