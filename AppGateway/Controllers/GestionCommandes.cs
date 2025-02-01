using System.Net;
using System.Text;
using System.Text.Json;
using GestionCommande.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGateway.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles="Admin,GestionnaireCommandes")]
    public class GestionCommandes : ControllerBase
    {
        private readonly HttpClient _gestionCommandesClient;
        private readonly HttpClient _gestionClientsClient;
        private readonly HttpClient _gestionStockClient;
        private readonly string _gestionCommandesUrl = "http://localhost:5012/api/Commandes/";
        private readonly string _gestionClientUrl = "http://localhost:5154/api/Client/";
        private readonly string _gestionStockUrl = "http://localhost:5071/api/Stock/";

        public GestionCommandes(IHttpClientFactory httpClientFactory)
        {
            _gestionCommandesClient = httpClientFactory.CreateClient("GestionCommandesClient");
        }

        // GET: api/<OrderingController>
        [HttpGet("Commandes")]
        public async Task<IResult> ListerCommandes()
        {
            var response = await _gestionCommandesClient.GetAsync(_gestionCommandesUrl);
            var content = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.ToString()??"application/json";
            return Results.Content(content, contentType,Encoding.UTF8, (int)response.StatusCode);
            // return StatusCode((int)response.StatusCode, content);
        }

        // GET api/<OrderingController>/5
        [HttpGet("Commandes/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> GetCommande(int id)
        {
            var response = await _gestionCommandesClient.GetAsync(_gestionCommandesUrl+id);
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        
        // [HttpPost("Clients/creerClient")]
        // public async Task<ActionResult<ClientResponseDTO>> CreerClientIfNotExists(ClientRequestDTO clientDto)
        // {
        //     var json = JsonSerializer.Serialize(clientDto);
        //     var content = new StringContent(json, Encoding.UTF8, "application/json");
        //     var response = await _httpClient.PostAsync(_GestionClientUrl, content);
        //     return StatusCode((int)response.StatusCode, content);
        // }

        // POST api/<OrderingController>
        [HttpPost("Commandes/nouvelleCommande")]
        public async Task<ActionResult<CommandeResponseDTO>> CreerCommande(CommandeRequestDTO commandeDto)
        {
            var json = JsonSerializer.Serialize(commandeDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _gestionCommandesClient.PostAsync(_gestionCommandesUrl, content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("Commandes/{idCommande}/AjouterArticle")]
        public async Task<ActionResult<CommandeResponseDTO>> AjouterArticle(int idCommande,
            ArticleCommandeRequestDTO articleCommandeRequestDto)
        {
            var json = JsonSerializer.Serialize(articleCommandeRequestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _gestionCommandesClient.PostAsync(_gestionCommandesUrl + "ajouterArticle/" + idCommande , content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        
        [HttpPost("Commandes/{idCommande}/RetirerArticle")]
        public async Task<ActionResult<CommandeResponseDTO>> RetirerArticle(int idCommande,
            ArticleCommandeRequestDTO articleCommandeRequestDto)
        {
            var json = JsonSerializer.Serialize(articleCommandeRequestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _gestionCommandesClient.PostAsync(_gestionCommandesUrl + "retirerArticle/" + idCommande, content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        
        [HttpPut("Commandes/{idCommande}/ModifierClient")]
        public async Task<ActionResult<CommandeResponseDTO>> ModifierClient(int idCommande, CommandeRequestDTO commandeRequestDto)
        {
            var json = JsonSerializer.Serialize(commandeRequestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _gestionCommandesClient.PutAsync(_gestionCommandesUrl + idCommande, content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        
        [HttpPost("Commandes/{idCommande}/ValiderCommande")]
        public async Task<ActionResult<CommandeResponseDTO>> ValiderCommande(int idCommande)
        {
            var response = await _gestionCommandesClient.PostAsync(_gestionCommandesUrl + "valider/" + idCommande, null);
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        
        [HttpPost("Commandes/{idCommande}/AnnulerCommande")]
        public async Task<ActionResult<CommandeResponseDTO>> AnnulerCommande(int idCommande)
        {
            var commandResponse = await _gestionCommandesClient.GetAsync($"{_gestionCommandesUrl}{idCommande}");
            if(!commandResponse.IsSuccessStatusCode)
            {
                return NotFound("Commande non trouvée");
            }
            var commandcontent = await commandResponse.Content.ReadAsStringAsync();
            String statusCommand = JsonSerializer.Deserialize<CommandeResponseDTO>(commandcontent).status;
            HttpResponseMessage? responseCommande;
            if (statusCommand == "PREPARATION")
            {
                responseCommande = await _gestionCommandesClient.DeleteAsync($"{_gestionCommandesUrl}{idCommande}");
                return StatusCode((int)responseCommande.StatusCode, await responseCommande.Content.ReadAsStringAsync());
            }
            responseCommande = await _gestionCommandesClient.DeleteAsync($"{_gestionCommandesUrl}annuler/{idCommande}");
            if (!responseCommande.IsSuccessStatusCode || statusCommand != "RESERVEE")
            {
                return StatusCode((int)responseCommande.StatusCode, await responseCommande.Content.ReadAsStringAsync());
            }
            
            var responseStock = await _gestionStockClient.DeleteAsync($"{_gestionStockUrl}annulerReservationCommande/{idCommande}");
            if (!responseStock.IsSuccessStatusCode)
            {
                await _gestionCommandesClient.PostAsync($"{_gestionCommandesUrl}Rollback/{idCommande}", 
                    new StringContent(JsonSerializer.Serialize(new { lastStatus = statusCommand }), Encoding.UTF8, "application/json"));
                return StatusCode((int)responseStock.StatusCode, await responseStock.Content.ReadAsStringAsync());
            }

            return StatusCode((int)HttpStatusCode.OK,
                $"{await responseStock.Content.ReadAsStringAsync()}\n{await responseCommande.Content.ReadAsStringAsync()}");
        }
        
        // [HttpPost("Commandes/{idCommande}/FacturerCommande")]
        // public async Task<ActionResult<CommandeResponseDTO>> FacturerCommande(int idCommande)
        // {
        //     var response = await _httpClient.PostAsync(_GestionCommandesUrl + "facturer/" + idCommande, null);
        //     var content = await response.Content.ReadAsStringAsync();
        //     return StatusCode((int)response.StatusCode, content);
        // }
        //
        
        //
        // [HttpPost("Commandes/{idCommande}/LivrerCommande")]
        // public async Task<ActionResult<CommandeResponseDTO>> LivrerCommande(int idCommande)
        // {
        //     var response = await _httpClient.PostAsync(_GestionCommandesUrl + "livrer/" + idCommande, null);
        //     var content = await response.Content.ReadAsStringAsync();
        //     return StatusCode((int)response.StatusCode, content);
        // }
    }
}