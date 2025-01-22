using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public interface IProduitRepo : IGenericRepository<Produit>
{
    Task<bool> ProduitExists(string nom);
    Task<bool> ProduitExists(int id);
}