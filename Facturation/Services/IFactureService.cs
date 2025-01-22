using Facturation.DTO;
using Persistence.entities.Facturation;

namespace Facturation.Services
{
    public interface IFactureService
    {
        Task<Facture?> ConsulterFacture(int id);
        Task<IEnumerable<Facture>> ConsulterFactures();
        Task<Facture> CreerFacture(CreerFactureDTO creerFactureDTO);
        Task<Facture?> SupprimerFacture(int factureId);
        Task<Facture> UpdateFacture(int factureId, UpdateFactureDTO updateFactureDTO);
        Task<Paiement> AjouterPaiement(int factureId, CreerPaiementDTO creerPaiementDTO);
        Task<IEnumerable<Paiement>> ConsulterPaiements(int factureId);
        Task<Paiement> CreePaiement(int factureId, double montant);
        Task<Paiement?> SupprimerPaiement(int paiementId);
        Task<Paiement> UpdatePaiement(int paiementId, UpdatePaiementDTO updatePaiementDTO);
    }
}