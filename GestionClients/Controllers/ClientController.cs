using GestionClients.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.entities.Client;

namespace GestionClients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService stockService)
        {
            _clientService = stockService;
        }
        [HttpPost("ajouterClient")]
        public IActionResult AjouterClient(Client client )
        {
            _clientService.ajouterClient(client);
            return CreatedAtAction("ConsulterClient", new {id = client.Id});
        }

        [HttpGet("consulterClient")]
        public IActionResult ConsulterClient(int id)
        {
            var client = _clientService.consulterClient(id);
            return Ok(client);
        }

        [HttpPut("modifierClient")]
        public async Task<IActionResult> ModifierClient([FromBody] Client client)
        {
            try
            {
                await _clientService.modifierClientAsync(client);
                return Ok(new { message = "Client modifié avec succès." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Une erreur est survenue lors de la modification du client.", details = ex.Message });
            }
        }
        [HttpGet("FiltrerClients")]
        public async Task<IActionResult> FiltrerClients([FromQuery] string? nom = null, [FromQuery] bool? estRestreint = null, [FromQuery] float? note = null)
        {
            try
            {
                Func<Client, bool> condition = client =>
                {
                    bool match = true;
                    if (!string.IsNullOrEmpty(nom))
                    {
                        match &= client.nom != null && client.nom.Contains(nom, StringComparison.OrdinalIgnoreCase);
                    }
                    if (estRestreint.HasValue)
                    {
                        match &= client.estRestreint == estRestreint.Value;
                    }
                    if (note.HasValue)
                    {
                        match &= client.note >= note.Value;
                    }

                    return match;
                };
                var filteredClients = await _clientService.FiltrerClients(condition);
                if (filteredClients == null || !filteredClients.Any())
                {
                    return NotFound(new { message = "Aucun client ne correspond aux critères fournis." });
                }
                return Ok(filteredClients);
            }
            catch (Exception ex)
            {
             
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Une erreur est survenue lors du filtrage des clients.", details = ex.Message });
            }
        }
        [HttpPut("evaluerClient")]
        public async Task<IActionResult> EvaluerClient(int id, int note)
        {
            try
            {
                await _clientService.evaluerClientAsync(id, note);
                return Ok(new { message = "Client évalué avec succès." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Une erreur est survenue lors de l'évaluation du client.", details = ex.Message });
            }
        }
        [HttpPatch("RestaurerClient/{id}")]
        public async Task<IActionResult> RestaurerClient(int id)
        {
            try
            {
                await _clientService.restaurerClient(id);
                return Ok(new { message = $"Le client avec l'ID {id} a été restauré avec succès." });
            }
            catch (KeyNotFoundException ex)
            { 
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Une erreur est survenue lors de la restauration du client.", details = ex.Message });
            }
        }

        [HttpPatch("RestreindreClient/{id}")]
        public async Task<IActionResult> RestreindreClient(int id)
        {
            try
            {
                await _clientService.restreindreClient(id);
                return Ok(new { message = $"Le client avec l'ID {id} a été restreint avec succès." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Une erreur est survenue lors de la restriction du client.", details = ex.Message });
            }
        }
        [HttpDelete("SupprimerClient/{id}")]
        public async Task<IActionResult> SupprimerClient(int id)
        {
            try
            {
                await _clientService.supprimerClient(id);
                return Ok(new { message = $"Le client avec l'ID {id} a été supprimé avec succès." });
            }
            catch (KeyNotFoundException ex) 
            { 
            
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Une erreur est survenue lors de la suppression du client.", details = ex.Message });
            }
        }



    }
}
