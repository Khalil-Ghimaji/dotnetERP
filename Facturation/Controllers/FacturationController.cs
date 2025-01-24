using Facturation.DTO;
using Facturation.Services;
using Microsoft.AspNetCore.Mvc;
using Persistence.entities.Facturation;

namespace Facturation.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FacturationController : ControllerBase
    {
        /*
        private readonly AppDbContext _context; // Remplacez AppDbContext par le nom de votre contexte

        public FacturationController(AppDbContext context)
        {
            _context = context;
        }*/
        private readonly IFactureService _factureService;
        
        public FacturationController(IFactureService factureService)
        {
            _factureService = factureService;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Facture>> ConsulterFacture(int id)
        {
            var facture = await _factureService.ConsulterFacture(id);
            if (facture == null)
                return NotFound("Facture non trouvée.");
            return Ok(facture);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Facture>>> ConsulterFactures()
        {
            var factures = await _factureService.ConsulterFactures();
            return Ok(factures);
        }

        [HttpPost]
        
        public async Task<ActionResult<Facture>> CreerFacture([FromBody] CreerFactureDTO creerFactureDTO)
        {
            if (creerFactureDTO == null)
                return BadRequest("Données de facture invalides.");
            
            var facture = await _factureService.CreerFacture(creerFactureDTO);
            return CreatedAtAction(nameof(ConsulterFacture), new { id = facture.FactureId }, facture);
        }
        /*
        public async Task<ActionResult<Facture>> CreerFacture([FromBody] CreerFactureDTO creerFactureDTO)
        {
            
            var facture = new Facture
            {
                CommandeId = creerFactureDTO.CommandeId,
                DateGeneration = DateTime.UtcNow,
                MontantTotal = creerFactureDTO.MontantTotal,
                StatusFacture = StatusFacture.Créée // ou une valeur par défaut si applicable
            };

            // Ajouter au contexte
            _context.Factures.Add(facture);

            // Enregistrer les changements
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ConsulterFacture), new { id = facture.FactureId }, facture);
        }*/

        [HttpDelete("{factureId}")]
        public async Task<ActionResult> SupprimerFacture(int factureId)
        {
            var facture = await _factureService.SupprimerFacture(factureId);
            if (facture == null)
                return NotFound("Facture non trouvée.");

            return NoContent();
        }

        [HttpPut("{factureId}")]
        public async Task<ActionResult<Facture>> UpdateFacture(int factureId, [FromBody] UpdateFactureDTO updateFactureDTO)
        {
            if (updateFactureDTO == null)
                return BadRequest("Données de mise à jour invalides.");

            var facture = await _factureService.UpdateFacture(factureId, updateFactureDTO);
            return Ok(facture);
        }
        

        [HttpPost("{factureId}/paiement")]
        public async Task<ActionResult<Paiement>> AjouterPaiement(int factureId, [FromBody] CreerPaiementDTO creerPaiementDTO)
        {
            var paiement = await _factureService.AjouterPaiement(factureId, creerPaiementDTO);
            return CreatedAtAction(nameof(ConsulterPaiement), new { factureId = factureId }, paiement);
        }

        [HttpGet("{factureId}/paiements")]
        public async Task<ActionResult<IEnumerable<Paiement>>> ConsulterPaiements(int factureId)
        {
            var paiements = await _factureService.ConsulterPaiements(factureId);
            return Ok(paiements);
        }

        [HttpGet("paiement/{paiementId}")]
        public async Task<ActionResult<Paiement>> ConsulterPaiement(int paiementId)
        {
            var paiement = await _factureService.ConsulterPaiement(paiementId);
            if (paiement == null)
                return NotFound("Aucun paiement trouvé.");
            
            return Ok(paiement);
        }

        [HttpDelete("paiement/{paiementId}")]
        public async Task<ActionResult> SupprimerPaiement(int paiementId)
        {
            var paiement = await _factureService.SupprimerPaiement(paiementId);
            if (paiement == null)
                return NotFound("Paiement non trouvé.");
            
            return NoContent();
        }

        [HttpPut("paiement/{paiementId}")]
        public async Task<ActionResult<Paiement>> UpdatePaiement(int paiementId, [FromBody] UpdatePaiementDTO updatePaiementDTO)
        {
            var paiement = await _factureService.UpdatePaiement(paiementId, updatePaiementDTO);
            if (paiement == null)
                return NotFound("Paiement non trouvé.");
            
            return Ok(paiement);
        }
        [HttpGet("{factureId}/pdf")]
        public async Task<IActionResult> GenererFacturePdf(int factureId)
        {
            try
            {
                var facturePdf = await _factureService.GenererFacturePdf(factureId);

                if (facturePdf == null)
                    return NotFound("Facture non trouvée ou problème lors de la génération du PDF.");

                return File(facturePdf, "application/pdf", $"Facture_{factureId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la génération du PDF : {ex.Message}");
            }
        }
        
        

    }
    
}
