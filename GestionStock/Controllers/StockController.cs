using GestionStock.DTO;
using GestionStock.Services;
using Microsoft.AspNetCore.Mvc;
using Persistence.entities.Commande;

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
        public async Task<IActionResult> AjouterProduit(ProduitDTO dto)
        {
            try
            {
                await _stockService.AjouterProduit(dto);
                return CreatedAtAction(nameof(ConsulterProduit), dto);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("ajouterQuantiteStock")]
        public IActionResult AjouterStock(ArticleStockDTO dto)
        {
            try
            {
                _stockService.AjouterQuantite(dto);
                return Created();
            }
            catch (Exception e)
            {
                return new NotFoundResult();
            }
        }

        [HttpGet("consulterProduit")]
        public async Task<IActionResult> ConsulterProduit(int id)
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
        public async Task<IActionResult> ConsulterStock()
        {
            var stock = await _stockService.ConsulterStock();
            return Ok(stock);
        }

        [HttpPost("expedierMarchandises")]
        public async Task<IActionResult> ExpedierMarchandises(Commande commande)
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
        public async Task<IActionResult> ReserverProduit(ReserverProduitDTO dto)
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

        [HttpPost("confirmerCommande")]
        public IActionResult ConfirmerCommande(Guid id)
        {
            _stockService.ConfirmerCommande(id);
            return NoContent();
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