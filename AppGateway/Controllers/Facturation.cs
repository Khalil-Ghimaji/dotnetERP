using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Persistence.DTO.Facturation;
using Persistence.entities.Commande;

namespace AppGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _facturationUrl = "http://localhost:5105/api/Facturation/"; 
        private readonly string _gestionCommandesUrl = "http://localhost:5012/api/Commandes/"; // URL mise à jour

        public FacturationController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FacturationClient");
        }

        [HttpGet("factures")]
        public async Task<IActionResult> ListerFactures()
        {
            var response = await _httpClient.GetAsync($"{_facturationUrl}");
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpGet("factures/{id}")]
        public async Task<IActionResult> ConsulterFacture(int id)
        {
            var response = await _httpClient.GetAsync($"{_facturationUrl}{id}");
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpPost("factures")]
        public async Task<IActionResult> CreerFacture([FromBody] CreerFactureDTO creerFactureDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var json = JsonSerializer.Serialize(creerFactureDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_facturationUrl}", content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPut("factures/{id}")]
        public async Task<IActionResult> ModifierFacture(int id, [FromBody] UpdateFactureDTO updateFactureDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var json = JsonSerializer.Serialize(updateFactureDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_facturationUrl}{id}", content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpDelete("factures/{id}")]
        public async Task<IActionResult> SupprimerFacture(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_facturationUrl}{id}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("factures/{factureId}/echeances")]
        public async Task<IActionResult> AjouterEcheance(int factureId, [FromBody] CreerEcheanceDTO creerEcheanceDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var json = JsonSerializer.Serialize(creerEcheanceDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_facturationUrl}{factureId}/echeance", content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpGet("factures/{factureId}/echeances")]
        public async Task<IActionResult> ConsulterEcheances(int factureId)
        {
            var response = await _httpClient.GetAsync($"{_facturationUrl}{factureId}/echeances");
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpGet("echeances/{echeanceId}")]
        public async Task<IActionResult> ConsulterEcheance(int echeanceId)
        {
            var response = await _httpClient.GetAsync($"{_facturationUrl}echeance/{echeanceId}");
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpPut("echeances/{echeanceId}")]
        public async Task<IActionResult> ModifierEcheance(int echeanceId, [FromBody] UpdateEcheanceDTO updateEcheanceDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var json = JsonSerializer.Serialize(updateEcheanceDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_facturationUrl}echeance/{echeanceId}", content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpDelete("echeances/{echeanceId}")]
        public async Task<IActionResult> SupprimerEcheance(int echeanceId)
        {
            var response = await _httpClient.DeleteAsync($"{_facturationUrl}echeance/{echeanceId}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpGet("factures/{factureId}/pdf")]
        public async Task<IActionResult> GenererFacturePdf(int factureId)
        {
            var response = await _httpClient.GetAsync($"{_facturationUrl}{factureId}/pdf");
            var content = await response.Content.ReadAsByteArrayAsync();
            return File(content, "application/pdf", $"Facture_{factureId}.pdf");
        }

        [HttpPost("factures/{factureId}/envoyer-email")]
        public async Task<IActionResult> EnvoyerFactureParEmail(int factureId, [FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("L'adresse e-mail est requise.");
            }

            var json = JsonSerializer.Serialize(new { Email = email });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_facturationUrl}{factureId}/envoyer-email", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok("Facture envoyée par e-mail avec succès.");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorContent);
            }
        }
        
        
        
        //lezm netfaker nhotha fi blasetha baad ki nthabet mel endpoint
        private async Task VerifierEtMettreAJourStatutCommande(int factureId, int commandeId)
        {
            try
            {
                var checkPaymentResponse = await _httpClient.GetAsync($"{_facturationUrl}{factureId}/est_payée");
                var checkPaymentContent = await checkPaymentResponse.Content.ReadAsStringAsync();
                bool isPaid = bool.Parse(checkPaymentContent); 

                if (isPaid)
                {
                    var orderStatusUpdate = new { Status = StatusCommande.PAYEE };  
                    var orderUpdateJson = JsonSerializer.Serialize(orderStatusUpdate);
                    var orderContent = new StringContent(orderUpdateJson, Encoding.UTF8, "application/json");

                    var orderResponse = await _httpClient.PutAsync($"{_gestionCommandesUrl}{commandeId}/update-status", orderContent);

                    if (!orderResponse.IsSuccessStatusCode)
                    {
                        throw new Exception("Échec de la mise à jour du statut de la commande.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur dans la vérification et la mise à jour du statut de la commande : {ex.Message}");
            }
        }

        }
    }
