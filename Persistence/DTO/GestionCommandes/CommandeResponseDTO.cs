using Persistence.entities.Client;
using Persistence.entities.Commande;
using Persistence.entities.Facturation;

namespace GestionCommande.DTOs;

public class CommandeResponseDTO
{
    public int Id { get; set; }
    public DateTime dateCommande { get; set; }
    public String status { get; set; }
    public ClientResponseDTO client { get; set; }
    public List<ArticleCommandeResponseDTO> articles { get; set; }
    public FactureResponseDTO Facture { get; set; }
}