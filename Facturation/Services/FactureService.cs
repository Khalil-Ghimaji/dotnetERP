using AutoMapper;
using Persistence.DTO.Facturation;
using Persistence.entities.Facturation;
using Persistence.Repository.CommandeRepositories;
using Persistence.Repository.FacturationRepositories;
using Persistence.entities.Commande;

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

            // Vérifier si l'état de la commande est bien "VALIDEE"
            if (commande.status != StatusCommande.VALIDEE)
            {
                throw new Exception($"Impossible de créer une facture pour une commande dont l'état est {commande.status}. La commande doit être VALIDEE.");
            }

            var facture = _mapper.Map<Facture>(creerFactureDTO);
            facture.Commande = commande;

            // Vérifier si MontantTotal est défini dans le DTO, sinon le calculer
            if (!creerFactureDTO.MontantTotal.HasValue) 
            {
                facture.MontantTotal = (float)commande.articles.Sum(article => article.prix * article.quantite);
            }
            else
            {
                facture.MontantTotal = creerFactureDTO.MontantTotal.Value;
            }

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

            var commande = await _commandeRepo.GetById(facture.CommandeId);
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
                throw new Exception(
                    "Le montant total des échéances dépasse le montant restant à payer pour cette facture.");
            }

            var echeance = _mapper.Map<Echeance>(creerEcheanceDto);
            echeance.FactureId = factureId;

            if (echeance.StatutEcheance == StatutEcheance.Payée)
            {
                facture.MontantPayé += echeance.Montant;
                await _factureRepo.Update(facture);
            }


            VerifEtatFacture(facture.FactureId);
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
            VerifEtatFacture(facture.FactureId);

            await _echeanceRepo.Delete(echeanceId);
        }

        public async Task<EcheanceResponseDTO> UpdateEcheance(int echeanceId, UpdateEcheanceDTO updateEcheanceDto)
        {
            var echeance = await _echeanceRepo.GetById(echeanceId);
            if (echeance == null) throw new Exception("Echeance non trouvé.");
            var facture = await _factureRepo.GetById(echeance.FactureId);
            if (facture == null) throw new Exception("Facture non trouvée.");

            var ancienMontant = echeance.Montant;
            var ancienStatut = echeance.StatutEcheance;
  

            var echeancesExistantes = await _echeanceRepo.GetEcheancesByFactureId(echeance.FactureId);

            var total = echeancesExistantes.Sum(e => e.Montant);

            var montantRestant = facture.MontantTotal - facture.MontantPayé;

            var montantTotalApresModification = total - ancienMontant + updateEcheanceDto.Montant;

            if (montantTotalApresModification > montantRestant)
            {
                throw new Exception(
                    "Le montant total des échéances dépasse le montant restant à payer pour cette facture.");
            }

            _mapper.Map(updateEcheanceDto, echeance);
            // Si l'échéance a été payée, on met à jour le montant payé de la facture en conséquence.

            if (echeance.StatutEcheance == StatutEcheance.Payée )
            {
                if (echeance.StatutEcheance == ancienStatut)
                {
                    facture.MontantPayé += echeance.Montant - ancienMontant;

                }
                else
                {
                    facture.MontantPayé += echeance.Montant ;
                }
                await _factureRepo.Update(facture);
            }
            // Si l'échéance est impayée et que le montant a diminué, on ajuste le montant payé de la facture.

            if (echeance.StatutEcheance != StatutEcheance.Payée && ancienStatut == StatutEcheance.Payée)
            {
                facture.MontantPayé -= ancienMontant;
                await _factureRepo.Update(facture);
            }

            VerifEtatFacture(facture.FactureId);
            var result = await _echeanceRepo.Update(echeance);
            return _mapper.Map<EcheanceResponseDTO>(result);
        }


        public async Task<byte[]> GenererFacturePdf(int factureId)
        {
            var facture = await _factureRepo.GetById(factureId);
            if (facture == null) throw new Exception("Facture non trouvée.");

            return _pdfService.GenererFacturePDF(facture);
        }

        public async Task EnvoyerFactureParEmail(int factureId)
        {
            var facture = await _factureRepo.GetById(factureId);
            if (facture == null) throw new Exception("Facture non trouvée.");

            var facturePdf = _pdfService.GenererFacturePDF(facture);

            var subject = $"Facture #{factureId} - Mini-ERP";
            var body = $@"
        <html>
        <body>
            <h2>Facture #{factureId}</h2>
            <p>Bonjour,</p>
            <p>Veuillez trouver ci-joint votre facture #{factureId}.</p>
            <p>Montant total : {facture.MontantTotal:C}</p>
            <p>Date de génération : {facture.DateGeneration:dd/MM/yyyy}</p>
            <br/>
            <p>Cordialement,<br/>Mini-ERP</p>
        </body>
        </html>";

            await _mailService.SendEmailAsync(facture.Commande.client.email, subject, body, facturePdf);
        }

        private async Task VerifEtatFacture(int factureId)
        {
            var facture = await _factureRepo.GetById(factureId);
            if (facture == null) throw new Exception("Facture non trouvée.");

            var echeancesExistantes = await _echeanceRepo.GetEcheancesByFactureId(factureId);

            var total = echeancesExistantes.Sum(e => e.Montant);

            Console.WriteLine("hhhhhhhhhhhhhhhhhhhhhhhh\nkkkkk");
            Console.WriteLine(total);
            Console.WriteLine("hhhhhhhhhhhhhhhhhhhhhhhh\nkkkkk");
            Console.WriteLine(facture.MontantTotal);

            if (total == facture.MontantTotal)
            {
                facture.StatusFacture = StatusFacture.Validée;
            }
            else
            {
                facture.StatusFacture = StatusFacture.Créée;
            }
            if (facture.MontantPayé == facture.MontantTotal)
            {
                facture.StatusFacture = StatusFacture.Payée;
            }

            await _factureRepo.Update(facture);
        }
    }
}