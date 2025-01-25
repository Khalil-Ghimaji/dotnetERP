using GestionCommande.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AppGateway.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GestionStockController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _gestionCommandesUrl = "https://localhost:5012/api/Commandes/";
    private readonly string _gestionStockUrl = "https://localhost:5071/api/Stock/";
    private readonly string _gestionClientUrl = "https://localhost:5154/api/Client/";
    private readonly string _gestionFacturationUrl = "https://localhost:5105/api/Facturation/";
        
    public GestionStockController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }
    
    [HttpPost("Commandes/{idCommande}/ExpedierCommande")]
    public async Task<ActionResult<CommandeResponseDTO>> ExpedierCommande(int idCommande)
    {
        
        var response = await _httpClient.PostAsync(_gestionCommandesUrl + "expedier/" + idCommande, null);
        var content = await response.Content.ReadAsStringAsync();
        return StatusCode((int)response.StatusCode, content);
    }
    
}