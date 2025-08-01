using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using GestionCommande.DTOs;
using GestionStock.DTO;
using Microsoft.AspNetCore.Authorization;
using Persistence;
using AjouterQuantiteRequestDTO = GestionStock.DTO.ArticleExpedierMarchandisesDTO;

namespace AppGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN,GESTIONNAIRE_STOCK")]
    public class GestionStock : ControllerBase
    {
        private readonly HttpClient _gestionStockClient;
        private readonly HttpClient _gestionCommandesClient;
        private readonly string _gestionStockUrl = "http://localhost:5071/api/Stock/";
        private readonly string _gestionCommandesUrl = "http://localhost:5012/api/Commandes/";

        public GestionStock(IHttpClientFactory httpClientFactory)
        {
            _gestionStockClient = httpClientFactory.CreateClient("GestionStockClient");
            _gestionCommandesClient = httpClientFactory.CreateClient("GestionCommandesClient");
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
        public async Task<IActionResult> ExpedierMarchandises(int CommandeId)
        {
            var commandResponse = await _gestionCommandesClient.GetAsync($"{_gestionCommandesUrl}{CommandeId}");
            if (!commandResponse.IsSuccessStatusCode)
            {
                return NotFound("Commande non trouvée");
            }

            var commandcontent = await commandResponse.Content.ReadAsStringAsync();
            var command = JsonSerializer.Deserialize<CommandeResponseDTO>(commandcontent);
            String statusCommand = command.status;

            var commandeResponse =
                await _gestionCommandesClient.PostAsync($"{_gestionCommandesUrl}expedier/{CommandeId}", null);
            if (!commandeResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)commandeResponse.StatusCode, await commandeResponse.Content.ReadAsStringAsync());
            }

            HttpResponseMessage stockResponse;
            if (statusCommand == "FACTUREE")
            {
                stockResponse = await _gestionStockClient.PostAsync($"{_gestionStockUrl}expedierMarchandises",
                    new StringContent(JsonSerializer.Serialize(new { idCommande = CommandeId }), Encoding.UTF8,
                        "application/json"));
            }
            else
            {
                stockResponse =
                    await _gestionStockClient.PostAsync($"{_gestionStockUrl}confirmerCommande/{CommandeId}", null);
            }

            if (!stockResponse.IsSuccessStatusCode)
            {
                await _gestionCommandesClient.PostAsync($"{_gestionCommandesUrl}rollback/{CommandeId}",
                    new StringContent(JsonSerializer.Serialize(new { lastStatus = statusCommand }), Encoding.UTF8,
                        "application/json"));
                return StatusCode((int)stockResponse.StatusCode, await stockResponse.Content.ReadAsStringAsync());
            }

            return StatusCode((int)HttpStatusCode.OK,
                $"{await stockResponse.Content.ReadAsStringAsync()}\n{await commandeResponse.Content.ReadAsStringAsync()}");
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
            var responseCommande =
                await _gestionCommandesClient.PostAsync($"{_gestionCommandesUrl}reserver/{dto.idCommande}", null);
            if (!responseCommande.IsSuccessStatusCode)
            {
                return StatusCode((int)responseCommande.StatusCode, await responseCommande.Content.ReadAsStringAsync());
            }

            var responseStock = await _gestionStockClient.PostAsync($"{_gestionStockUrl}reserverCommande",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
            if (!responseStock.IsSuccessStatusCode)
            {
                await _gestionCommandesClient.PostAsync($"{_gestionCommandesUrl}rollback/{dto.idCommande}",
                    new StringContent(JsonSerializer.Serialize(new { lastStatus = "FACTUREE" }), Encoding.UTF8,
                        "application/json"));
                return StatusCode((int)responseStock.StatusCode, await responseStock.Content.ReadAsStringAsync());
            }

            return StatusCode((int)HttpStatusCode.OK,
                $"{await responseStock.Content.ReadAsStringAsync()}\n{await responseCommande.Content.ReadAsStringAsync()}");
        }

        [HttpDelete("annulerReservationCommande/{commandeId}")]
        public async Task<IActionResult> AnnulerReservationCommande(int commandeId)
        {
            var commandResponse = await _gestionCommandesClient.GetAsync($"{_gestionCommandesUrl}{commandeId}");
            if (!commandResponse.IsSuccessStatusCode)
            {
                return NotFound("Commande non trouvée");
            }

            var commandcontent = await commandResponse.Content.ReadAsStringAsync();
            String statusCommand = JsonSerializer.Deserialize<CommandeResponseDTO>(commandcontent).status;

            var responseCommande =
                await _gestionCommandesClient.DeleteAsync($"{_gestionCommandesUrl}annuler/{commandeId}");
            if (!responseCommande.IsSuccessStatusCode)
            {
                return StatusCode((int)responseCommande.StatusCode, await responseCommande.Content.ReadAsStringAsync());
            }

            var responseStock =
                await _gestionStockClient.DeleteAsync($"{_gestionStockUrl}annulerCommande/{commandeId}");
            if (!responseStock.IsSuccessStatusCode)
            {
                await _gestionCommandesClient.PostAsync($"{_gestionCommandesUrl}rollback/{commandeId}",
                    new StringContent(JsonSerializer.Serialize(new { lastStatus = statusCommand }), Encoding.UTF8,
                        "application/json"));
                return StatusCode((int)responseStock.StatusCode, await responseStock.Content.ReadAsStringAsync());
            }

            return StatusCode((int)HttpStatusCode.OK,
                $"{await responseStock.Content.ReadAsStringAsync()}\n{await responseCommande.Content.ReadAsStringAsync()}");
        }

        [HttpDelete("supprimerProduit")]
        public async Task<IActionResult> SupprimerProduit(int produitId)
        {
            var response =
                await _gestionStockClient.DeleteAsync($"{_gestionStockUrl}supprimerProduit?produitId={produitId}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}