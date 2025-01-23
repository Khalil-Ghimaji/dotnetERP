using Persistence.entities.Stock;
using Persistence.Repository;

namespace GestionStock.Repository;

public interface IProduitRepo : IGenericRepository<Produit>
{
    //chercher un produit par son nom ou son id
    Task<bool> ProduitExists(int id, string nom);
}