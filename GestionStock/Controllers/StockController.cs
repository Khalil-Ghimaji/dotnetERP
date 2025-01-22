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
        public IActionResult AjouterProduit(ProduitDTO dto)
        {
            try
            {
                _stockService.AjouterProduit(dto);
                return Created();
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
        public IActionResult ConsulterProduit(int id)
        {
            try
            {
                var produit = _stockService.ConsulterProduit(id);
                return Ok(produit);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("consulterStock")]
        public IActionResult ConsulterStock()
        {
            var stock = _stockService.ConsulterStock();
            return Ok(stock);
        }

        [HttpPost("expedierMarchandises")]
        public IActionResult ExpedierMarchandises(Commande commande)
        {
            try
            {
                _stockService.ExpedierMarchandises(commande);
                return NoContent();
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPut("modifierProduit")]
        public IActionResult ModifierProduit(ProduitDTO dto)
        {
            try
            {
                _stockService.ModifierProduit(dto);
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
                await _stockService.ReserverProduit(dto);
                return NoContent();
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPost("confirmerCommande")]
        public IActionResult ConfirmerCommande(int id)
        {
            _stockService.ConfirmerCommande(id);
            return NoContent();
        }

        [HttpDelete("supprimerProduit")]
        public IActionResult SupprimerProduit(int id)
        {
            try
            {
                _stockService.SupprimerProduit(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}