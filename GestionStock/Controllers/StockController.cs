using GestionStock.DTO;
using GestionStock.Services;
using Microsoft.AspNetCore.Mvc;
using AjouterQuantiteRequestDTO = GestionStock.DTO.ArticleExpedierMarchandisesDTO;

namespace GestionStock.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost("ajouterProduit")]
        public async Task<ActionResult<AjouterProduitResponseDTO>> AjouterProduit(AjouterProduitRequestDTO dto)
        {
            try
            {
                var id = await _stockService.AjouterProduit(dto);
                return CreatedAtAction(nameof(ConsulterProduit), new { id }, new AjouterProduitResponseDTO()
                {
                    Id = id
                });
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("ajouterQuantiteStock")]
        public async Task<IActionResult> AjouterStock(AjouterQuantiteRequestDTO dto)
        {
            try
            {
                await _stockService.AjouterQuantite(dto);
                return Created();
            }
            catch (Exception e)
            {
                return new NotFoundResult();
            }
        }

        [HttpGet("consulterProduit")]
        public async Task<ActionResult<ArticleStockDTO>> ConsulterProduit(int id)
        {
            try
            {
                var produit = await _stockService.ConsulterProduit(id);
                return Ok(produit);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("consulterStock")]
        public async Task<ActionResult<List<ArticleStockDTO>>> ConsulterStock()
        {
            return Ok(await _stockService.ConsulterStock());
        }

        [HttpPost("expedierMarchandises")]
        public async Task<IActionResult> ExpedierMarchandises(ExpedierMarchandisesRequestDTO commande)
        {
            try
            {
                await _stockService.ExpedierMarchandises(commande);
                return NoContent();
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPut("modifierProduit")]
        public async Task<IActionResult> ModifierProduit(ProduitDTO dto)
        {
            try
            {
                await _stockService.ModifierProduit(dto);
                return NoContent();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("reserverProduit")]
        public async Task<ActionResult<ReserverProduitResponseDTO>> ReserverProduit(ReserverProduitRequestDTO dto)
        {
            try
            {
                var reservationId = await _stockService.ReserverProduit(dto);
                return Ok(new { ReservationId = reservationId });
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpDelete("annulerCommande")]
        public async Task<IActionResult> AnnulerCommande(Guid id)
        {
            try
            {
                await _stockService.AnnulerCommande(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("confirmerCommande")]
        public IActionResult ConfirmerCommande(Guid id)
        {
            try
            {
                _stockService.ConfirmerCommande(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("supprimerProduit")]
        public async Task<IActionResult> SupprimerProduit(int id)
        {
            try
            {
                await _stockService.SupprimerProduit(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}