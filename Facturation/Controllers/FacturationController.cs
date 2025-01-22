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

        [HttpGet("{factureId}/paiement")]
        public async Task<ActionResult<Paiement>> ConsulterPaiement(int factureId)
        {
            var paiement = await _factureService.ConsulterPaiements(factureId);
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
    }
}
