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
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
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
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
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
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
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
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
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
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
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
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpDelete("annulerCommande")]
        public async Task<IActionResult> AnnulerCommande(Guid reservationId)
        {
            try
            {
                await _stockService.AnnulerCommande(reservationId);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPost("confirmerCommande")]
        public IActionResult ConfirmerCommande(ConfirmerCommandeRequestDTO dto)
        {
            try
            {
                _stockService.ConfirmerCommande(dto.ReservationId);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpDelete("supprimerProduit")]
        public async Task<IActionResult> SupprimerProduit(int produitId)
        {
            try
            {
                await _stockService.SupprimerProduit(produitId);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }
    }
}