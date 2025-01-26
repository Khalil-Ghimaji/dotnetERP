using Microsoft.AspNetCore.Mvc;
using Persistence.DTO.Facturation;
using Facturation.Services;

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
        public async Task<ActionResult<FactureResponseDTO>> ConsulterFacture(int id)
        {
            var facture = await _factureService.ConsulterFacture(id);
            if (facture == null) return NotFound("Facture non trouvée.");
            return Ok(facture);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FactureResponseDTO>>> ConsulterFactures()
        {
            var factures = await _factureService.ConsulterFactures();
            return Ok(factures);
        }

        [HttpPost]
        public async Task<ActionResult<FactureResponseDTO>> CreerFacture([FromBody] CreerFactureDTO creerFactureDTO)
        {
            if (creerFactureDTO == null) return BadRequest("Données de facture invalides.");

            var facture = await _factureService.CreerFacture(creerFactureDTO);
            return CreatedAtAction(nameof(ConsulterFacture), new { id = facture.FactureId }, facture);
        }

        [HttpDelete("{factureId}")]
        public async Task<ActionResult> SupprimerFacture(int factureId)
        {
            await _factureService.SupprimerFacture(factureId);
            return NoContent();
        }

        [HttpPut("{factureId}")]
        public async Task<ActionResult<FactureResponseDTO>> UpdateFacture(int factureId, [FromBody] UpdateFactureDTO updateFactureDTO)
        {
            if (updateFactureDTO == null) return BadRequest("Données de mise à jour invalides.");

            var facture = await _factureService.UpdateFacture(factureId, updateFactureDTO);
            return Ok(facture);
        }

        [HttpPost("{factureId}/echeance")]
        public async Task<ActionResult<EcheanceResponseDTO>> AjouterEcheance(int factureId, [FromBody] CreerEcheanceDTO creerEcheanceDto)
        {
            var echeance = await _factureService.AjouterEcheance(factureId, creerEcheanceDto);
            return CreatedAtAction(nameof(ConsulterEcheance), new { echeanceId = echeance.EcheanceId }, echeance);
        }

        [HttpGet("{factureId}/echeances")]
        public async Task<ActionResult<IEnumerable<EcheanceResponseDTO>>> ConsulterEcheances(int factureId)
        {
            var echeances = await _factureService.ConsulterEcheances(factureId);
            return Ok(echeances);
        }

        [HttpGet("echeance/{echeanceId}")]
        public async Task<ActionResult<EcheanceResponseDTO>> ConsulterEcheance(int echeanceId)
        {
            var echeance = await _factureService.ConsulterEcheance(echeanceId);
            if (echeance == null) return NotFound("Aucun échéance trouvé.");
            return Ok(echeance);
        }

        [HttpDelete("echeance/{echeanceId}")]
        public async Task<ActionResult> SupprimerEcheance(int echeanceId)
        {
            await _factureService.SupprimerEcheance(echeanceId);
            return NoContent();
        }

        [HttpPut("echeance/{echeanceId}")]
        public async Task<ActionResult<EcheanceResponseDTO>> UpdateEcheance(int echeanceId, [FromBody] UpdateEcheanceDTO updateEcheanceDto)
        {
            var echeance = await _factureService.UpdateEcheance(echeanceId, updateEcheanceDto);
            if (echeance == null) return NotFound("Échéance non trouvée.");
            return Ok(echeance);
        }

        [HttpGet("{factureId}/pdf")]
        public async Task<IActionResult> GenererFacturePdf(int factureId)
        {
            try
            {
                var facturePdf = await _factureService.GenererFacturePdf(factureId);
                return File(facturePdf, "application/pdf", $"Facture_{factureId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la génération du PDF : {ex.Message}");
            }
        }

        [HttpPost("{factureId}/envoyer-email")]
        public async Task<IActionResult> EnvoyerFactureParEmail(int factureId, [FromBody] string email)
        {
            try
            {
                await _factureService.EnvoyerFactureParEmail(factureId, email);
                return Ok("Facture envoyée par e-mail avec succès.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'envoi de l'e-mail : {ex.Message}");
            }
        }
    }
}