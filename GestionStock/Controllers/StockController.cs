using System.Text.Json;
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
        public async Task<IActionResult> ExpedierMarchandises([FromBody]dynamic body)
        {
            int idCommande = JsonSerializer.Deserialize<JsonElement>(body).GetProperty("idCommande").GetInt32();
            try
            {
                await _stockService.ExpedierMarchandises(idCommande);
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

        [HttpPost("reserverCommande")]
        public async Task<ActionResult> ReserverCommande(ReserverCommandeRequestDTO dto)
        {
            try
            {
                await _stockService.ReserverCommande(dto);
                return Ok();
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

        [HttpDelete("annulerCommande/{commandeId}")]
        public async Task<IActionResult> AnnulerCommande(int commandeId)
        {
            try
            {
                await _stockService.AnnulerCommande(commandeId);
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

        [HttpPost("confirmerCommande/{commandeId}")]
        public IActionResult ConfirmerCommande(int commandeId)
        {
            try
            {
                _stockService.ConfirmerCommande(commandeId);
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