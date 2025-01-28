using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.DTO.GestionClients;
using System.Text;
using System.Text.Json;

namespace AppGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionClientsLayer2 : ControllerBase
    {
            private readonly HttpClient _httpClient;
            private const string Layer3BaseUrl = "http://localhost:5154/api/Client";
            public GestionClientsLayer2(IHttpClientFactory httpClientFactory)
            {
                _httpClient = httpClientFactory.CreateClient();
            }
        [HttpGet("tester")]
        public IActionResult Tester()
        {
            return Ok("GestionClientsLayer2 est fonctionnel.");
        }

            [HttpPost("ajouterClient")]
            public async Task<IActionResult> AjouterClient([FromBody] ClientIn client)
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(client), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{Layer3BaseUrl}/ajouterClient", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return Created("", await response.Content.ReadAsStringAsync());
                }

                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            [HttpGet("consulterClient/{id}")]
            public async Task<IActionResult> ConsulterClient(int id)
            {
                var response = await _httpClient.GetAsync($"{Layer3BaseUrl}/consulterClient?id={id}");

                if (response.IsSuccessStatusCode)
                {
                    var client = await response.Content.ReadAsStringAsync();
                    return Ok(client);
                }

                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            [HttpGet("listerClients")]
            public async Task<IActionResult> ListerClients()
            {
                var response = await _httpClient.GetAsync($"{Layer3BaseUrl}/listerClients");

                if (response.IsSuccessStatusCode)
                {
                    var clients = await response.Content.ReadAsStringAsync();
                    return Ok(clients);
                }

                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            [HttpPut("modifierClient/{id}")]
            public async Task<IActionResult> ModifierClient(int id, [FromBody] ClientIn client)
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(client), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{Layer3BaseUrl}/modifierClient?id={id}", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Client modifié avec succès." });
                }

                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            [HttpDelete("SupprimerClient/{id}")]
            public async Task<IActionResult> SupprimerClient(int id)
            {
                var response = await _httpClient.DeleteAsync($"{Layer3BaseUrl}/SupprimerClient/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = $"Le client avec l'ID {id} a été supprimé avec succès." });
                }

                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            [HttpGet("FiltrerClients")]
            public async Task<IActionResult> FiltrerClients([FromQuery] string? nom = null, [FromQuery] bool? estRestreint = null, [FromQuery] float? note = null, [FromQuery] string? adresse = null)
            {
                var queryString = $"?nom={nom}&estRestreint={estRestreint}&note={note}&adresse={adresse}";
                var response = await _httpClient.GetAsync($"{Layer3BaseUrl}/FiltrerClients{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var clients = await response.Content.ReadAsStringAsync();
                    return Ok(clients);
                }

                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            [HttpPatch("RestaurerClient/{id}")]
            public async Task<IActionResult> RestaurerClient(int id)
            {
                var response = await _httpClient.PatchAsync($"{Layer3BaseUrl}/RestaurerClient/{id}", null);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = $"Le client avec l'ID {id} a été restauré avec succès." });
                }

                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            [HttpPatch("RestreindreClient/{id}")]
            public async Task<IActionResult> RestreindreClient(int id)
            {
                var response = await _httpClient.PatchAsync($"{Layer3BaseUrl}/RestreindreClient/{id}", null);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = $"Le client avec l'ID {id} a été restreint avec succès." });
                }

                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            [HttpPut("evaluerClient/{id}/{note}")]
            public async Task<IActionResult> EvaluerClient(int id, int note)
            {
                var response = await _httpClient.PutAsync($"{Layer3BaseUrl}/evaluerClient?id={id}&note={note}", null);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Client évalué avec succès." });
                }

                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
    }

