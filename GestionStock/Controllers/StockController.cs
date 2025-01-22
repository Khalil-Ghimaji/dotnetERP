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
        public IActionResult AjouterProduit(int produitId, int quantite)
        {
            _stockService.AjouterProduit(produitId, quantite);
            return Created();
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
        public IActionResult ModifierProduit(int id, int quantite)
        {
            _stockService.ModifierProduit(id, quantite);
            return Ok();
        }

        [HttpPost("reserverProduit")]
        public async Task<IActionResult> ReserverProduit(int id, int quantite, TimeSpan reservationDuration)
        {
            await _stockService.ReserverProduit(id, quantite, reservationDuration);
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