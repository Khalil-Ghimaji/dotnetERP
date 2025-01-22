using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public CommandesController(ICommandeService commandeService)
        {
            _commandeService = commandeService;
        }

        // GET: api/Commandes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Commande>>> GetCommandes()
        {
            var commandes = await _commandeService.getAllCommandes();
            return commandes.ToList();
        }

        // GET: api/Commandes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Commande>> GetCommande(int id)
        {
            var commande = await _commandeService.getCommandeById(id);

            if (commande == null)
            {
                return NotFound();
            }

            return commande;
        }

        // PUT: api/Commandes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifierCommande(int id, Commande commande)
        {
            if (id != commande.Id)
            {
                return BadRequest();
            }
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound();
            }
            var command = await _commandeService.modifierCommande(commande);
            if (command == null)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // POST: api/Commandes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Commande>> CreerCommande(Commande commande)
        {
            var command = await _commandeService.preparerCommande(commande);
            if (command == null)
            {
                return BadRequest();
            }

            return CreatedAtAction("GetCommande", new { id = commande.Id }, commande);
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
        public async Task<IActionResult> AjouterArticle(int idCommande, ArticleCommandeDTO articleCommande)
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
            return NoContent();
        }
        
        [HttpPost("retirerArticle/{idCommande}")]
        public async Task<IActionResult> RetirerArticle(int idCommande, ArticleCommandeDTO articleCommande)
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
            return NoContent();
        }
        
        [HttpPost("valider/{id}")]
        public async Task<ActionResult<Commande>> ValiderCommande(int id)
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
            return commande;
        }
        
        [HttpPost("annuler/{id}")]
        public async Task<ActionResult<Commande>> AnnulerCommande(int id)
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
            return commande;
        }

        [HttpPost("expedier/{id}")]
        public async Task<ActionResult<Commande>> ExpedierCommande(int id)
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
            return commande;
        }

        [HttpPost("livrer/{id}")]
        public async Task<ActionResult<Commande>> LivrerCommande(int id)
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
            return commande;
        }

        [HttpPost("facturer/{id}")]
        public async Task<ActionResult<Commande>> FacturerCommande(int id)
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
            return commande;
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
