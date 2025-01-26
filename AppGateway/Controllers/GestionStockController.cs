// using System.Text;
// using System.Text.Json;
// using GestionCommande.DTOs;
// using GestionStock.DTO;
// using Microsoft.AspNetCore.Mvc;
//
// namespace AppGateway.Controllers;
//
// [Route("api/[controller]")]
// [ApiController]
// public class GestionStockController : ControllerBase
// {
//     private readonly HttpClient _client;
//     private readonly string _gestionCommandesUrl = "https://localhost:5012/api/Commandes/";
//     private readonly string _gestionStockUrl = "https://localhost:5071/api/Stock/";
//     private readonly string _gestionClientUrl = "https://localhost:5154/api/Client/";
//     private readonly string _gestionFacturationUrl = "https://localhost:5105/api/Facturation/";
//         
//     public GestionStockController(IHttpClientFactory httpClientFactory)
//     {
//         _client = httpClientFactory.CreateClient();
//     }
//     
//     [HttpPost("ajouterProduit")]
//         public async Task<IActionResult> AjouterProduit(AjouterProduitRequestDTO dto)
//         {
//             var response = await _client.PostAsync($"{_gestionStockUrl}/ajouterProduit",
//                 new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
//             return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
//         }
//
//         [HttpPost("ajouterQuantiteStock")]
//         public async Task<IActionResult> AjouterStock(AjouterQuantiteRequestDTO dto)
//         {
//             var response = await _client.PostAsync($"{_gestionStockUrl}/ajouterQuantiteStock",
//                 new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
//             return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
//         }
//
//         [HttpGet("consulterProduit")]
//         public async Task<IActionResult> ConsulterProduit(int id)
//         {
//             var response = await _client.GetAsync($"{_gestionStockUrl}/consulterProduit?id={id}");
//             return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
//         }
//
//         [HttpGet("consulterStock")]
//         public async Task<IActionResult> ConsulterStock()
//         {
//             var response = await _client.GetAsync($"{_gestionStockUrl}/consulterStock");
//             return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
//         }
//
//         [HttpPost("expedierMarchandises")]
//         public async Task<IActionResult> ExpedierMarchandises(ExpedierMarchandisesRequestDTO commande)
//         {
//             var response = await _client.PostAsync($"{_gestionStockUrl}/expedierMarchandises",
//                 new StringContent(JsonSerializer.Serialize(commande), Encoding.UTF8, "application/json"));
//             return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
//         }
//
//         [HttpPut("modifierProduit")]
//         public async Task<IActionResult> ModifierProduit(ProduitDTO dto)
//         {
//             var response = await _client.PutAsync($"{_gestionStockUrl}/modifierProduit",
//                 new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
//             return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
//         }
//
//         [HttpPost("reserverProduit")]
//         public async Task<IActionResult> ReserverProduit(ReserverProduitRequestDTO dto)
//         {
//             var response = await _client.PostAsync($"{_gestionStockUrl}/reserverProduit",
//                 new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
//             return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
//         }
//
//         [HttpDelete("annulerCommande")]
//         public async Task<IActionResult> AnnulerCommande(Guid reservationId)
//         {
//             var response = await _client.DeleteAsync($"{_gestionStockUrl}/annulerCommande?reservationId={reservationId}");
//             return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
//         }
//
//         [HttpPost("confirmerCommande")]
//         public async Task<IActionResult> ConfirmerCommande(Guid reservationId)
//         {
//             var requestDto = new ConfirmerCommandeRequestDTO { ReservationId = reservationId };
//             var response = await _client.PostAsync($"{_gestionStockUrl}/confirmerCommande",
//                 new StringContent(JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json"));
//             return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
//         }
//
//         [HttpDelete("supprimerProduit")]
//         public async Task<IActionResult> SupprimerProduit(int produitId)
//         {
//             var response = await _client.DeleteAsync($"{_gestionStockUrl}/supprimerProduit?produitId={produitId}");
//             return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
//         }
//     }
//     
// }