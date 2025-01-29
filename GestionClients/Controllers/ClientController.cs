using GestionClients.Services;
using Microsoft.AspNetCore.Mvc;
using Persistence.DTO.GestionClients;
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
        public async Task<IActionResult> AjouterClient(ClientIn client)
        {
            try
            {
                await _clientService.ajouterClient(client);
                var clientOut = await _clientService.listerClients();
                foreach (var c in clientOut)
                {
                    if (c.nom == client.nom && c.telephone == client.telephone && c.address == client.address)
                    {
                        return CreatedAtAction("ConsulterClient", new { id = c.Id }, c);
                    }
                }

                return BadRequest(new { message = "Aucun client correspondant trouvé après l'ajout." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
            }
        }


        [HttpGet("consulterClient")]
        public async Task<ActionResult> ConsulterClient(int id)
        {
            var client = await _clientService.consulterClient(id);
            return client == null ? NotFound("Client non trouvé.") : Ok(client);
        }

        [HttpGet("listerClients")]
        public async Task<IActionResult> ListerClients()
        {
            try
            {
                var clients = await _clientService.listerClients();
                if (clients == null || !clients.Any())
                {
                    return NotFound("Aucun client trouvé.");
                }

                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Une erreur s'est produite : {ex.Message}");
            }
        }

        [HttpPut("modifierClient")]
        public async Task<IActionResult> ModifierClient(int id, ClientIn client)
        {
            try
            {
                await _clientService.modifierClient(id, client.nom, client.address, client.telephone);
                return Ok(new { message = "Client modifié avec succès." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "Une erreur est survenue lors de la modification du client.", details = ex.Message
                    });
            }
        }

        [HttpGet("FiltrerClients")]
        public async Task<IActionResult> FiltrerClients([FromQuery] string? nom = null,
            [FromQuery] bool? estRestreint = null, [FromQuery] float? note = null, [FromQuery] string? adresse = null)
        {
            try
            {
                Func<Client, bool> condition = client =>
                {
                    bool match = true;
                    if (!string.IsNullOrEmpty(nom))
                    {
                        match &= client.nom.Contains(nom, StringComparison.OrdinalIgnoreCase);
                    }

                    if (estRestreint.HasValue)
                    {
                        match &= client.estRestreint == estRestreint.Value;
                    }

                    if (note.HasValue)
                    {
                        match &= client.note >= note.Value;
                    }

                    if (!string.IsNullOrEmpty(adresse))
                    {
                        match &=
                            client.address.Contains(adresse, StringComparison.OrdinalIgnoreCase);
                    }

                    return match;
                };
                var filteredClients = await _clientService.filtrerClients(condition);
                if (!filteredClients.Any())
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
                await _clientService.evaluerClient(id, note);
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
                    new
                    {
                        message = "Une erreur est survenue lors de la restauration du client.", details = ex.Message
                    });
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
                    new
                    {
                        message = "Une erreur est survenue lors de la restriction du client.", details = ex.Message
                    });
            }
        }

        /*[HttpDelete("SupprimerClient/{id}")]
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
                    new
                    {
                        message = "Une erreur est survenue lors de la suppression du client.", details = ex.Message
                    });
            }
        }*/
    }
}