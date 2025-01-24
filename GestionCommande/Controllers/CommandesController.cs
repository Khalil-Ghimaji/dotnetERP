using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GestionCommande.DTOs;
using GestionCommande.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using Persistence;
using Persistence.entities.Commande;

namespace GestionCommande.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandesController : ControllerBase
    {
        private readonly ICommandeService _commandeService;
        private readonly IMapper _mapper;

        public CommandesController(ICommandeService commandeService, IMapper mapper)
        {
            _commandeService = commandeService;
            _mapper = mapper;
        }

        // GET: api/Commandes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandeResponseDTO>>> GetCommandes()
        {
            var commandes = await _commandeService.getAllCommandes();
            
            return _mapper.Map<List<CommandeResponseDTO>>(commandes.ToList());
        }

        // GET: api/Commandes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> GetCommande(int id)
        {
            var commande = await _commandeService.getCommandeById(id);

            if (commande == null)
            {
                return NotFound();
            }

            return _mapper.Map<CommandeResponseDTO>(commande);
        }

        // PUT: api/Commandes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> ModifierCommande(int id, CommandeRequestDTO commandeDto)
        {
            // if (id != commande.Id)
            // {
            //     return BadRequest();
            // }
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound();
            }

            var command = _mapper.Map<Commande>(commandeDto);
            command.Id = id;
            command = await _commandeService.modifierCommande(command);
            if (command == null)
            {
                return BadRequest();
            }
            return _mapper.Map<CommandeResponseDTO>(command);
        }

        // POST: api/Commandes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CommandeResponseDTO>> CreerCommande(CommandeRequestDTO commandeDto)
        {
            var command = await _commandeService.preparerCommande(_mapper.Map<Commande>(commandeDto));
            if (command == null)
            {
                return NotFound();
            }

            return CreatedAtAction("GetCommande", new { id = command.Id }, _mapper.Map<CommandeResponseDTO>(command));
        }

        // DELETE: api/Commandes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommande(int id)
        {
            var commande = await _commandeService.supprimerCommande(id);
            if (commande == null)
            {
                return NotFound();
            }
            return NoContent();
        }
        
        [HttpPost("ajouterArticle/{idCommande}")]
        public async Task<ActionResult<CommandeResponseDTO>> AjouterArticle(int idCommande, ArticleCommandeRequestDTO articleCommande)
        {
            if (!await _commandeService.commandeExists(idCommande))
            {
                return NotFound();
            }
            var commande = await _commandeService.ajouterArticle(idCommande, articleCommande.IdProduit, articleCommande.Quantite);
            if (commande == null)
            {
                return BadRequest();
            }
            return _mapper.Map<CommandeResponseDTO>(commande);
        }
        
        [HttpPost("retirerArticle/{idCommande}")]
        public async Task<ActionResult<CommandeResponseDTO>> RetirerArticle(int idCommande, ArticleCommandeRequestDTO articleCommande)
        {
            if (!await _commandeService.commandeExists(idCommande))
            {
                return NotFound();
            }
            var commande = await _commandeService.retirerArticle(idCommande, articleCommande.IdProduit, articleCommande.Quantite);
            if (commande == null)
            {
                return BadRequest();
            }
            return _mapper.Map<CommandeResponseDTO>(commande);
        }
        
        [HttpPost("valider/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> ValiderCommande(int id)
        {
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound();
            }
            var commande = await _commandeService.validerCommande(id);
            if (commande == null)
            {
                return BadRequest();
            }
            return _mapper.Map<CommandeResponseDTO>(commande);
        }
        
        [HttpPost("annuler/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> AnnulerCommande(int id)
        {
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound();
            }
            var commande = await _commandeService.annulerCommande(id);
            if(commande == null)
            {
                return BadRequest();
            }
            return _mapper.Map<CommandeResponseDTO>(commande);
        }

        [HttpPost("expedier/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> ExpedierCommande(int id)
        {
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound();
            }
            var commande = await _commandeService.expedierCommande(id);
            if (commande == null)
            {
                return BadRequest();
            }
            return _mapper.Map<CommandeResponseDTO>(commande);
        }

        [HttpPost("livrer/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> LivrerCommande(int id)
        {
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound();
            }
            
            var commande = await _commandeService.livrerCommande(id);
            if (commande == null)
            {
                return BadRequest();
            }
            return _mapper.Map<CommandeResponseDTO>(commande);
        }

        [HttpPost("facturer/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> FacturerCommande(int id)
        {
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound();
            }
            var commande = await _commandeService.facturerCommande(id);
            if (commande == null)
            {
                return BadRequest();
            }
            return _mapper.Map<CommandeResponseDTO>(commande);
        }

        // [HttpPost("payer/{id}")]
        // public async Task<ActionResult<Commande>> PayerCommande(int id)
        // {
        //     var commande = await _commandeService.getCommandeById(id);
        //     if (commande == null)
        //     {
        //         return NotFound();
        //     }
        //     _commandeService.payerCommande(commande);
        //     return commande;
        // }

        // [HttpPost("rembourser/{id}")]
        // public async Task<ActionResult<Commande>> RembourserCommande(int id)
        // {
        //     var commande = await _commandeService.getCommandeById(id);
        //     if (commande == null)
        //     {
        //         return NotFound();
        //     }
        //     commande.status = StatusCommande.REMBOURSEE;
        //     await _context.SaveChangesAsync();
        //     return commande;
        // }

        // [HttpPost("retourner/{id}")]
        // public async Task<ActionResult<Commande>> RetournerCommande(int id)
        // {
        //     var commande = await _commandeService.getCommandeById(id);
        //     if (commande == null)
        //     {
        //         return NotFound();
        //     }
        //     commande.status = StatusCommande.RETOURNEE;
        //     await _context.SaveChangesAsync();
        //     return commande;
        // }
    }
}
