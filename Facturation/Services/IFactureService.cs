using Persistence.DTO.Facturation;
using Persistence.entities.Facturation;

namespace Facturation.Services
{
    public interface IFactureService
    {
        Task<FactureResponseDTO?> ConsulterFacture(int id);
        Task<IEnumerable<FactureResponseDTO>> ConsulterFactures();
        Task<FactureResponseDTO> CreerFacture(CreerFactureDTO creerFactureDTO);
        Task SupprimerFacture(int factureId);
        Task<FactureResponseDTO> UpdateFacture(int factureId, UpdateFactureDTO updateFactureDTO);
        Task<EcheanceResponseDTO> AjouterEcheance(int factureId, CreerEcheanceDTO creerEcheanceDto);
        Task<IEnumerable<EcheanceResponseDTO>> ConsulterEcheances(int factureId);
        Task<EcheanceResponseDTO> ConsulterEcheance(int echeanceId);
        Task SupprimerEcheance(int echeanceId);
        Task<EcheanceResponseDTO> UpdateEcheance(int echeanceId, UpdateEcheanceDTO updateEcheanceDto);
        Task<byte[]> GenererFacturePdf(int factureId);
        Task EnvoyerFactureParEmail(int factureId);
    }
}