using AutoMapper;
using Persistence.DTO.Facturation;
using Persistence.entities.Facturation;
using Persistence.Repository.CommandeRepositories;
using Persistence.Repository.FacturationRepositories;

namespace Facturation.Services
{
    public class FactureService : IFactureService
    {
        private readonly IFactureRepo _factureRepo;
        private readonly IEcheanceRepo _echeanceRepo;
        private readonly IPDFService _pdfService;
        private readonly ICommandeRepo _commandeRepo;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        public FactureService(
            IFactureRepo factureRepo,
            IEcheanceRepo echeanceRepo,
            ICommandeRepo commandeRepo,
            IPDFService pdfService,
            IMailService mailService,
            IMapper mapper)
        {
            _factureRepo = factureRepo;
            _echeanceRepo = echeanceRepo;
            _commandeRepo = commandeRepo;
            _pdfService = pdfService;
            _mailService = mailService;
            _mapper = mapper;
        }

        public async Task<FactureResponseDTO?> ConsulterFacture(int id)
        {
            var facture = await _factureRepo.GetById(id);
            return _mapper.Map<FactureResponseDTO>(facture);
        }

        public async Task<IEnumerable<FactureResponseDTO>> ConsulterFactures()
        {
            var factures = await _factureRepo.GetAll();
            return _mapper.Map<IEnumerable<FactureResponseDTO>>(factures);
        }

        public async Task<FactureResponseDTO> CreerFacture(CreerFactureDTO creerFactureDTO)
        {
            var commande = await _commandeRepo.GetById(creerFactureDTO.CommandeId);
            if (commande == null)
            {
                throw new Exception($"Commande avec ID {creerFactureDTO.CommandeId} introuvable.");
            }

            var facture = _mapper.Map<Facture>(creerFactureDTO);
            facture.Commande = commande;

            var result = await _factureRepo.Add(facture);
            return _mapper.Map<FactureResponseDTO>(result);
        }

        public async Task SupprimerFacture(int id)
        {
            var facture = await _factureRepo.GetById(id);
            if (facture == null) return;

            var echeances = await _echeanceRepo.GetEcheancesByFactureId(id);
            foreach (var echeance in echeances)
            {
                await _echeanceRepo.Delete(echeance.EcheanceId);
            }

            var commande = await _commandeRepo.getEagerById(facture.CommandeId);
            if (commande != null)
            {
                commande.Facture = null;
                await _commandeRepo.Update(commande);
            }

            await _factureRepo.Delete(id);
        }

        public async Task<FactureResponseDTO> UpdateFacture(int factureId, UpdateFactureDTO updateFactureDTO)
        {
            var facture = await _factureRepo.GetById(factureId);
            if (facture == null) throw new Exception("Facture non trouvée.");

            _mapper.Map(updateFactureDTO, facture);
            var result = await _factureRepo.Update(facture);
            return _mapper.Map<FactureResponseDTO>(result);
        }

        public async Task<EcheanceResponseDTO> AjouterEcheance(int factureId, CreerEcheanceDTO creerEcheanceDto)
        {
            var facture = await _factureRepo.GetById(factureId);
            if (facture == null) throw new Exception("Facture non trouvée.");

            var echeancesExistantes = await _echeanceRepo.GetEcheancesByFactureId(factureId);

            var totalPayé = echeancesExistantes.Sum(e => e.Montant);

            var montantRestant = facture.MontantTotal - facture.MontantPayé;
            if (creerEcheanceDto.Montant + totalPayé > montantRestant)
            {
                throw new Exception("Le montant total des échéances dépasse le montant restant à payer pour cette facture.");
            }

            var echeance = _mapper.Map<Echeance>(creerEcheanceDto);
            echeance.FactureId = factureId;

            if (echeance.StatutEcheance == StatutEcheance.Payée)
            {
                facture.MontantPayé += echeance.Montant;
                await _factureRepo.Update(facture);
            }

            if (facture.MontantPayé == facture.MontantTotal)
            {
                facture.StatusFacture = StatusFacture.Payée;
                await _factureRepo.Update(facture);
            }

            var result = await _echeanceRepo.Add(echeance);
            return _mapper.Map<EcheanceResponseDTO>(result);
        }


        public async Task<IEnumerable<EcheanceResponseDTO>> ConsulterEcheances(int factureId)
        {
            var echeances = await _echeanceRepo.GetEcheancesByFactureId(factureId);
            return _mapper.Map<IEnumerable<EcheanceResponseDTO>>(echeances);
        }

        public async Task<EcheanceResponseDTO> ConsulterEcheance(int echeanceId)
        {
            var echeance = await _echeanceRepo.GetById(echeanceId);
            return _mapper.Map<EcheanceResponseDTO>(echeance);
        }

        public async Task SupprimerEcheance(int echeanceId)
        {
            var echeance = await _echeanceRepo.GetById(echeanceId);
            if (echeance == null)
            {
                throw new Exception("Échéance non trouvée.");
            }

            var facture = await _factureRepo.GetById(echeance.FactureId);
            if (facture == null)
            {
                throw new Exception("Facture associée non trouvée.");
            }

            if (echeance.StatutEcheance == StatutEcheance.Payée)
            {
                facture.MontantPayé -= echeance.Montant;
                await _factureRepo.Update(facture);
            }

            await _echeanceRepo.Delete(echeanceId);
        }

        public async Task<EcheanceResponseDTO> UpdateEcheance(int echeanceId, UpdateEcheanceDTO updateEcheanceDto)
{
    var echeance = await _echeanceRepo.GetById(echeanceId);
    if (echeance == null) throw new Exception("Echeance non trouvé.");

    var facture = await _factureRepo.GetById(echeance.FactureId);
    if (facture == null) throw new Exception("Facture non trouvée.");

    var ancienMontant = echeance.Montant;

    var echeancesExistantes = await _echeanceRepo.GetEcheancesByFactureId(echeance.FactureId);

    var totalPayé = echeancesExistantes.Sum(e => e.Montant);

    var montantRestant = facture.MontantTotal - facture.MontantPayé;

    var montantTotalApresModification = totalPayé - ancienMontant + updateEcheanceDto.Montant;

    if (montantTotalApresModification > montantRestant)
    {
        throw new Exception("Le montant total des échéances dépasse le montant restant à payer pour cette facture.");
    }

    _mapper.Map(updateEcheanceDto, echeance);

    if (echeance.StatutEcheance == StatutEcheance.Payée)
    {
        facture.MontantPayé += echeance.Montant - ancienMontant;
        await _factureRepo.Update(facture);
    }
    else if (ancienMontant > echeance.Montant && echeance.StatutEcheance == StatutEcheance.Impayée)
    {
        facture.MontantPayé -= (ancienMontant - echeance.Montant);
        await _factureRepo.Update(facture);
    }

    if (facture.MontantPayé == facture.MontantTotal)
    {
        facture.StatusFacture = StatusFacture.Payée;
        await _factureRepo.Update(facture);
    }

    var result = await _echeanceRepo.Update(echeance);
    return _mapper.Map<EcheanceResponseDTO>(result);
}


        public async Task<byte[]> GenererFacturePdf(int factureId)
        {
            var facture = await _factureRepo.GetById(factureId);
            if (facture == null) throw new Exception("Facture non trouvée.");

            return _pdfService.GenererFacturePDF(facture);
        }

        public async Task EnvoyerFactureParEmail(int factureId, string email)
        {
            var facturePdf = await GenererFacturePdf(factureId);

            var subject = $"Facture #{factureId}";
            var body = "Veuillez trouver ci-joint votre facture.";

            await _mailService.SendEmailAsync(email, subject, body, facturePdf);
        }
    }
}
































