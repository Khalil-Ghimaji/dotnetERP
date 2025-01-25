using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using GestionStock.DTO;
using AjouterQuantiteRequestDTO = GestionStock.DTO.ArticleExpedierMarchandisesDTO;

namespace AppGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionStock : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly string _url = "http://localhost:5071/api/Stock";

        public GestionStock(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("GestionStockClient");
        }

        [HttpPost("ajouterProduit")]
        public async Task<IActionResult> AjouterProduit(AjouterProduitRequestDTO dto)
        {
            var response = await _client.PostAsync($"{_url}/ajouterProduit",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("ajouterQuantiteStock")]
        public async Task<IActionResult> AjouterStock(AjouterQuantiteRequestDTO dto)
        {
            var response = await _client.PostAsync($"{_url}/ajouterQuantiteStock",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpGet("consulterProduit")]
        public async Task<IActionResult> ConsulterProduit(int id)
        {
            var response = await _client.GetAsync($"{_url}/consulterProduit?id={id}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpGet("consulterStock")]
        public async Task<IActionResult> ConsulterStock()
        {
            var response = await _client.GetAsync($"{_url}/consulterStock");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("expedierMarchandises")]
        public async Task<IActionResult> ExpedierMarchandises(ExpedierMarchandisesRequestDTO commande)
        {
            var response = await _client.PostAsync($"{_url}/expedierMarchandises",
                new StringContent(JsonSerializer.Serialize(commande), Encoding.UTF8, "application/json"));
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPut("modifierProduit")]
        public async Task<IActionResult> ModifierProduit(ProduitDTO dto)
        {
            var response = await _client.PutAsync($"{_url}/modifierProduit",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("reserverProduit")]
        public async Task<IActionResult> ReserverProduit(ReserverProduitRequestDTO dto)
        {
            var response = await _client.PostAsync($"{_url}/reserverProduit",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpDelete("annulerCommande")]
        public async Task<IActionResult> AnnulerCommande(Guid reservationId)
        {
            var response = await _client.DeleteAsync($"{_url}/annulerCommande?reservationId={reservationId}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("confirmerCommande")]
        public async Task<IActionResult> ConfirmerCommande(Guid reservationId)
        {
            var requestDto = new ConfirmerCommandeRequestDTO { ReservationId = reservationId };
            var response = await _client.PostAsync($"{_url}/confirmerCommande",
                new StringContent(JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json"));
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpDelete("supprimerProduit")]
        public async Task<IActionResult> SupprimerProduit(int produitId)
        {
            var response = await _client.DeleteAsync($"{_url}/supprimerProduit?produitId={produitId}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}