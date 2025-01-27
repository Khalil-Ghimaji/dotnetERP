using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using GestionCommande.DTOs;
using GestionStock.DTO;
using Persistence;
using AjouterQuantiteRequestDTO = GestionStock.DTO.ArticleExpedierMarchandisesDTO;

namespace AppGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionStock : ControllerBase
    {
        private readonly HttpClient _gestionStockClient;
        private readonly HttpClient _gestionCommandesClient;
        private readonly string _gestionStockUrl = "http://localhost:5071/api/Stock/";
        private readonly string _gestionCommandesUrl = "http://localhost:5012/api/Commandes/";
        private readonly AppDbContext _context;

        public GestionStock(IHttpClientFactory httpClientFactory, AppDbContext context)
        {
            _gestionStockClient = httpClientFactory.CreateClient("GestionStockClient");
            _gestionCommandesClient = httpClientFactory.CreateClient("GestionCommandesClient");
            _context = context;
        }

        [HttpPost("ajouterProduit")]
        public async Task<IActionResult> AjouterProduit(AjouterProduitRequestDTO dto)
        {
            var response = await _gestionStockClient.PostAsync($"{_gestionStockUrl}ajouterProduit",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("ajouterQuantiteStock")]
        public async Task<IActionResult> AjouterStock(AjouterQuantiteRequestDTO dto)
        {
            var response = await _gestionStockClient.PostAsync($"{_gestionStockUrl}ajouterQuantiteStock",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpGet("consulterProduit")]
        public async Task<IActionResult> ConsulterProduit(int id)
        {
            var response = await _gestionStockClient.GetAsync($"{_gestionStockUrl}consulterProduit?id={id}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpGet("consulterStock")]
        public async Task<IActionResult> ConsulterStock()
        {
            var response = await _gestionStockClient.GetAsync($"{_gestionStockUrl}consulterStock");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("expedierMarchandises")]
        public async Task<IActionResult> ExpedierMarchandises(ExpedierMarchandisesRequestDTO commande, int CommandeId)
        {
            var commandResponse = await _gestionCommandesClient.GetAsync($"{_gestionCommandesUrl}{CommandeId}");
            if(!commandResponse.IsSuccessStatusCode)
            {
                return NotFound("Commande non trouvée");
            }
            var commandcontent = await commandResponse.Content.ReadAsStringAsync();
            String statusCommande = JsonSerializer.Deserialize<CommandeResponseDTO>(commandcontent).status;
            await using var transaction = await _context.Database.BeginTransactionAsync();
            Task<HttpResponseMessage>[] responseTasks;
            if(statusCommande == "FACTUREE"){
                responseTasks = new[]
                {
                    _gestionStockClient.PostAsync($"{_gestionStockUrl}expedierMarchandises",
                        new StringContent(JsonSerializer.Serialize(commande), Encoding.UTF8, "application/json")),
                    _gestionCommandesClient.PostAsync($"{_gestionCommandesUrl}expedier/{CommandeId}", null)
                };
            }
            else
            {
                responseTasks = new[]
                {
                    _gestionStockClient.PostAsync($"{_gestionStockUrl}confirmerCommande/{CommandeId}", null),
                    _gestionCommandesClient.PostAsync($"{_gestionCommandesUrl}expedier/{CommandeId}", null)
                };
            }
            var responses = await Task.WhenAll(responseTasks);
            var response1 = responses[0];
            var response2 = responses[1];
            if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                await transaction.CommitAsync();
                return StatusCode((int)HttpStatusCode.OK,
                    await response2.Content.ReadAsStringAsync());
            }

            if (response1.IsSuccessStatusCode && !response2.IsSuccessStatusCode)
            {
                await transaction.RollbackAsync();
                return StatusCode((int)response2.StatusCode,
                    await response2.Content.ReadAsStringAsync());
            }

            if (!response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                await transaction.RollbackAsync();
                return StatusCode((int)response1.StatusCode,
                    await response1.Content.ReadAsStringAsync());
            }

            await transaction.RollbackAsync();
            return StatusCode((int)HttpStatusCode.BadRequest,
                await response1.Content.ReadAsStringAsync() + "\n" + await response2.Content.ReadAsStringAsync());
        }

        [HttpPut("modifierProduit")]
        public async Task<IActionResult> ModifierProduit(ProduitDTO dto)
        {
            var response = await _gestionStockClient.PutAsync($"{_gestionStockUrl}modifierProduit",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("reserverCommande")]
        public async Task<IActionResult> ReserverCommande(ReserverCommandeRequestDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var responseTasks = new[]
            {
                _gestionStockClient.PostAsync($"{_gestionStockUrl}reserverCommande",
                    new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")),
                _gestionCommandesClient.PostAsync($"{_gestionCommandesUrl}reserver/{dto.idCommande}", null)
            };
            var responses = await Task.WhenAll(responseTasks);
            var response1 = responses[0];
            var response2 = responses[1];
            if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                await transaction.CommitAsync();
                return StatusCode((int)HttpStatusCode.OK,
                    await response2.Content.ReadAsStringAsync());
            }
            if (response1.IsSuccessStatusCode && !response2.IsSuccessStatusCode)
            {
                await transaction.RollbackAsync();
                return StatusCode((int)response2.StatusCode,
                    await response2.Content.ReadAsStringAsync());
            }
            if (!response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                await transaction.RollbackAsync();
                return StatusCode((int)response1.StatusCode,
                    await response1.Content.ReadAsStringAsync());
            }
            await transaction.RollbackAsync();
            return StatusCode((int)HttpStatusCode.BadRequest,
                await response1.Content.ReadAsStringAsync() + "\n" + await response2.Content.ReadAsStringAsync());
        }

        [HttpDelete("annulerCommande/{commandeId}")]
        public async Task<IActionResult> AnnulerCommande(int commandeId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var responseTasks = new[]
            {
                _gestionStockClient.DeleteAsync($"{_gestionStockUrl}annulerCommande/{commandeId}"),
                _gestionCommandesClient.DeleteAsync($"{_gestionCommandesUrl}annuler/{commandeId}")
            };
            var responses = await Task.WhenAll(responseTasks);
            var response1 = responses[0];
            var response2 = responses[1];
            if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                await transaction.CommitAsync();
                return StatusCode((int)HttpStatusCode.OK,
                    await response2.Content.ReadAsStringAsync());
            }
            if (response1.IsSuccessStatusCode && !response2.IsSuccessStatusCode)
            {
                await transaction.RollbackAsync();
                return StatusCode((int)response2.StatusCode,
                    await response2.Content.ReadAsStringAsync());
            }
            if (!response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                await transaction.RollbackAsync();
                return StatusCode((int)response1.StatusCode,
                    await response1.Content.ReadAsStringAsync());
            }
            await transaction.RollbackAsync();
            return StatusCode((int)HttpStatusCode.BadRequest,
                await response1.Content.ReadAsStringAsync() + "\n" + await response2.Content.ReadAsStringAsync());
        }

        // [HttpPost("confirmerCommande/{commandeId}")]
        // public async Task<IActionResult> ConfirmerCommande(int commandeId)
        // {
        //     var response =
        //         await _gestionStockClient.PostAsync($"{_gestionStockUrl}confirmerCommande/{commandeId}", null);
        //     return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        // }

        [HttpDelete("supprimerProduit")]
        public async Task<IActionResult> SupprimerProduit(int produitId)
        {
            var response =
                await _gestionStockClient.DeleteAsync($"{_gestionStockUrl}supprimerProduit?produitId={produitId}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}