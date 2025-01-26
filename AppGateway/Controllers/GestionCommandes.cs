using System.Text;
using System.Text.Json;
using GestionCommande.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AppGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionCommandes : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _gestionCommandesUrl = "http://localhost:5012/api/Commandes/";
        private readonly string _gestionClientUrl = "http://localhost:5154/api/Client/";

        public GestionCommandes(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GestionCommandesClient");
        }

        // GET: api/<OrderingController>
        [HttpGet("Commandes")]
        public async Task<IResult> ListerCommandes()
        {
            var response = await _httpClient.GetAsync(_gestionCommandesUrl);
            var content = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.ToString()??"application/json";
            return Results.Content(content, contentType,Encoding.UTF8, (int)response.StatusCode);
            // return StatusCode((int)response.StatusCode, content);
        }

        // GET api/<OrderingController>/5
        [HttpGet("Commandes/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> GetCommande(int id)
        {
            var response = await _httpClient.GetAsync(_gestionCommandesUrl+id);
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
            var response = await _httpClient.PostAsync(_gestionCommandesUrl, content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("Commandes/ajouterProduit/{idCommande}")]
        public async Task<ActionResult<CommandeResponseDTO>> AjouterProduit(int idCommande,
            ArticleCommandeRequestDTO articleCommandeRequestDto)
        {
            var json = JsonSerializer.Serialize(articleCommandeRequestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_gestionCommandesUrl + "ajouterArticle/" + idCommande , content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        
        [HttpPost("Commandes/{idCommande}/RetirerArticle")]
        public async Task<ActionResult<CommandeResponseDTO>> RetirerArticle(int idCommande,
            ArticleCommandeRequestDTO articleCommandeRequestDto)
        {
            var json = JsonSerializer.Serialize(articleCommandeRequestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_gestionCommandesUrl + "retirerArticle/" + idCommande, content);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        
        [HttpPost("Commandes/{idCommande}/ValiderCommande")]
        public async Task<ActionResult<CommandeResponseDTO>> ValiderCommande(int idCommande)
        {
            var response = await _httpClient.PostAsync(_gestionCommandesUrl + "valider/" + idCommande, null);
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        
        [HttpPost("Commandes/{idCommande}/AnnulerCommande")]
        public async Task<ActionResult<CommandeResponseDTO>> AnnulerCommande(int idCommande)
        {
            var response = await _httpClient.PostAsync(_gestionCommandesUrl + "annuler/" + idCommande, null);
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
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