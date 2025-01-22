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

        public IActionResult CreerProduit(CreerProduitDTO dto)
        {
            _stockService.CreerProduit(dto);
            return Created();
        }
        
        [HttpPost("ajouterProduit")]
        public IActionResult AjouterProduit(CreerProduitDTO dto)
        {
            try
            {
                _stockService.AjouterProduit(dto);
                return Created();
            }
            catch (Exception _)
            {
                return NotFound();
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
            _stockService.ExpedierMarchandises(commande);
            return Ok();
        }

        [HttpPut("modifierProduit")]
        public IActionResult ModifierProduit(ModifierProduitDTO dto)
        {
            _stockService.ModifierProduit(dto);
            return Ok();
        }

        [HttpPost("reserverProduit")]
        public async Task<IActionResult> ReserverProduit(ReserverProduitDTO dto)
        {
            await _stockService.ReserverProduit(dto);
            return Ok();
        }

        [HttpPost("confirmerCommande")]
        public IActionResult ConfirmerCommande(int id)
        {
            _stockService.ConfirmerCommande(id);
            return Ok();
        }

        [HttpDelete("supprimerProduit")]
        public IActionResult SupprimerProduit(int id)
        {
            _stockService.SupprimerProduit(id);
            return Ok();
        }
    }
}