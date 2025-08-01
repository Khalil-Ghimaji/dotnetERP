using Microsoft.AspNetCore.Mvc;
using Persistence.DTO.Facturation;
using Facturation.Services;
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
            if (creerFactureDTO == null)
            {
                return BadRequest("Données de facture invalides.");
            }

            try
            {
                var facture = await _factureService.CreerFacture(creerFactureDTO);
                return CreatedAtAction(nameof(ConsulterFacture), new { id = facture.FactureId }, facture);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                if (ex.InnerException is Microsoft.Data.Sqlite.SqliteException sqliteEx &&
                    sqliteEx.SqliteErrorCode == 19)
                {
                    return Conflict("La commande existe déjà avec ce même ID.");
                }

                return StatusCode(500, "Une erreur est survenue lors de la création de la facture.");
            }
            catch (Exception ex) when (ex.Message.Contains("Impossible de créer une facture"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{factureId}")]
        public async Task<ActionResult> SupprimerFacture(int factureId)
        {
            await _factureService.SupprimerFacture(factureId);
            return NoContent();
        }

        [HttpPut("{factureId}")]
        public async Task<ActionResult<FactureResponseDTO>> UpdateFacture(int factureId,
            [FromBody] UpdateFactureDTO updateFactureDTO)
        {
            if (updateFactureDTO == null) return BadRequest("Données de mise à jour invalides.");

            var facture = await _factureService.UpdateFacture(factureId, updateFactureDTO);
            return Ok(facture);
        }

        [HttpPost("{factureId}/echeance")]
        public async Task<ActionResult<EcheanceResponseDTO>> AjouterEcheance(int factureId,
            [FromBody] CreerEcheanceDTO creerEcheanceDto)
        {
            try
            {
                var echeance = await _factureService.AjouterEcheance(factureId, creerEcheanceDto);
                return CreatedAtAction(nameof(ConsulterEcheance), new { echeanceId = echeance.EcheanceId }, echeance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
            try
            {
                var echeance = await _factureService.ConsulterEcheance(echeanceId);
                return Ok(echeance);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("echeance/{echeanceId}")]
        public async Task<ActionResult> SupprimerEcheance(int echeanceId)
        {
            await _factureService.SupprimerEcheance(echeanceId);
            return NoContent();
        }

        [HttpPut("echeance/{echeanceId}")]
        public async Task<ActionResult<EcheanceResponseDTO>> UpdateEcheance(int echeanceId,
            [FromBody] UpdateEcheanceDTO updateEcheanceDto)
        {
            try
            {
                var echeance = await _factureService.UpdateEcheance(echeanceId, updateEcheanceDto);
                if (echeance == null) return NotFound("Échéance non trouvée.");
                return Ok(echeance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
        public async Task<IActionResult> EnvoyerFactureParEmail(int factureId)
        {
            try
            {
                await _factureService.EnvoyerFactureParEmail(factureId);
                return Ok("Facture envoyée par e-mail avec succès.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'envoi de l'e-mail : {ex.Message}");
            }
        }

        [HttpGet("{factureId}/est_payée")]
        public async Task<ActionResult<bool>> VerifierPaiementFacture(int factureId)
        {
            return await VerifierStatutFacture(factureId, StatusFacture.Payée);
        }

        [HttpGet("{factureId}/est_validée")]
        public async Task<ActionResult<bool>> VerifierValiditeFacture(int factureId)
        {
            return await VerifierStatutFacture(factureId, StatusFacture.Validée);
        }

        private async Task<ActionResult<bool>> VerifierStatutFacture(int factureId, StatusFacture statutAttendu)
        {
            try
            {
                var facture = await _factureService.ConsulterFacture(factureId);
                if (facture == null)
                {
                    return NotFound("Facture non trouvée.");
                }

                bool statutCorrespondant = facture.StatusFacture == statutAttendu.ToString();
                return Ok(statutCorrespondant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur : {ex.Message}");
            }
        }
    }
}