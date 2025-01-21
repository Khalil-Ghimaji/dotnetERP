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
            return Ok();
        }

        [HttpGet("consulterClient")]
        public IActionResult ConsulterClient(int id)
        {
            var client = _clientService.consulterClient(id);
            return Ok(client);
        }
        [HttpDelete("supprimerClient")]
        public IActionResult SupprimerClient(int id)
        {
            _clientService.supprimerClient(id);
            return Ok();
        }

        [HttpPut("modifierClient")]
        public IActionResult ModifierClient(Client client )
        {
            _clientService.modifierClient(client);
            return Ok();
        }
        [HttpPut("evaluerClient")]
        public IActionResult EvaluerClient(int id, int note)
        {
            _clientService.evaluerClient(id, note);
            return Ok();
        }
        [HttpPut("restaurerClient")]
        public IActionResult RestaurerClient(int id)
        {
            _clientService.restaurerClient(id);
            return Ok();
        }
        [HttpPut("restreindreClient")]
        public IActionResult RestreindreClient(int id)
        {
            _clientService.restreindreClient(id);
            return Ok();
        }

    }
}
