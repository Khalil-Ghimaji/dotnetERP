using Facturation.DTO;
using Persistence.entities.Facturation;
using Persistence.Repository.FacturationRepositories;

namespace Facturation.Services
{
    public class FactureService : IFactureService
    {
        private readonly IFactureRepo _factureRepo;
        private readonly IPaiementRepo _paiementRepo;
        private readonly IPDFService _pdfService;



        public FactureService(IFactureRepo factureRepo, IPaiementRepo paiementRepo,IPDFService pdfService)
        {
            _factureRepo = factureRepo;
            _paiementRepo = paiementRepo;
            _pdfService = pdfService;
        }

        public async Task<Facture?> ConsulterFacture(int id)
        {
            return await _factureRepo.GetById(id);
        }

        public async Task<IEnumerable<Facture>> ConsulterFactures()
        {
            return await _factureRepo.GetAll();
        }

        public async Task<Facture> CreerFacture(CreerFactureDTO creerFactureDTO)
        {
            
            var facture = new Facture
            {
                CommandeId = creerFactureDTO.CommandeId,
                DateGeneration = DateTime.Now,
                MontantTotal = creerFactureDTO.MontantTotal,
                StatusFacture = StatusFacture.Créée
            };

            return await _factureRepo.Add(facture);
        }



        public async Task<Facture?> SupprimerFacture(int id)
        {
            return await _factureRepo.Delete(id);
        }

        public async Task<Facture> UpdateFacture(int factureId, UpdateFactureDTO updateFactureDTO)
        {
            var facture = await _factureRepo.GetById(factureId);
            if (facture == null) throw new Exception("Facture non trouvée.");

            facture.MontantTotal = updateFactureDTO.MontantTotal ?? facture.MontantTotal;
            facture.StatusFacture = updateFactureDTO.StatusFacture ?? facture.StatusFacture;
            return await _factureRepo.Update(facture);
        }

        public async Task<Paiement> AjouterPaiement(int factureId, CreerPaiementDTO creerPaiementDTO)
        {
            var facture = await _factureRepo.GetById(factureId);
            if (facture == null) throw new Exception("Facture non trouvée.");

            var paiement = new Paiement
            {
                FactureId = factureId,
                Date = DateTime.Now,
                Montant = creerPaiementDTO.Montant,
                MethodePaiement = creerPaiementDTO.MethodePaiement
            };

            facture.MontantTotal += paiement.Montant;
            await _factureRepo.Update(facture);

            return await _paiementRepo.Add(paiement);
        }

        public async Task<IEnumerable<Paiement>> ConsulterPaiements(int factureId)
        {
            return await _paiementRepo.GetPaiementsByFactureId(factureId);
        }
        public async Task<Paiement> ConsulterPaiement(int paiementId)
        {
            return await _paiementRepo.GetById(paiementId);
        }

        public async Task<Paiement> CreePaiement(int factureId, double montant)
        {
            var paiement = new Paiement
            {
                Date = DateTime.Now,
                Montant = (float)montant,
                MethodePaiement = MethodePaiement.Virement
            };

            return await _paiementRepo.Add(paiement);
        }

        public async Task<Paiement?> SupprimerPaiement(int paiementId)
        {
            return await _paiementRepo.Delete(paiementId);
        }

        public async Task<Paiement> UpdatePaiement(int paiementId, UpdatePaiementDTO updatePaiementDTO)
        {
            var paiement = await _paiementRepo.GetById(paiementId);
            if (paiement == null) throw new Exception("Paiement non trouvé.");

            paiement.Montant = updatePaiementDTO.Montant ?? paiement.Montant;
            paiement.MethodePaiement = updatePaiementDTO.MethodePaiement ?? paiement.MethodePaiement;

            return await _paiementRepo.Update(paiement);
        }
        public async Task<byte[]> GenererFacturePdf(int factureId)
        {
            var facture = await _factureRepo.GetById(factureId);
            if (facture == null)
            {
                throw new Exception("Facture non trouvée.");
            }

            return _pdfService.GenererFacturePDF(facture); 
        }
        
    }
}