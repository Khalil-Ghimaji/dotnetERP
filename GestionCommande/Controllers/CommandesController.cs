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
                return NotFound($"Commande n{id} n'existe pas");
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
                return NotFound($"Commande n{id} n'existe pas");
            }

            var command = _mapper.Map<Commande>(commandeDto);
            command.Id = id;
            try
            {
                command = await _commandeService.modifierCommande(command);
                return _mapper.Map<CommandeResponseDTO>(command);
            }
            catch (HttpRequestException e)
            {
                return NotFound(e.Message);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/Commandes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CommandeResponseDTO>> CreerCommande(CommandeRequestDTO commandeDto)
        {
            try
            {
                var command = await _commandeService.preparerCommande(_mapper.Map<Commande>(commandeDto));
                return CreatedAtAction("GetCommande", new { id = command.Id },
                    _mapper.Map<CommandeResponseDTO>(command));
            }
            catch (HttpRequestException e)
            {
                return NotFound(e.Message);
            }
        }

        // DELETE: api/Commandes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommande(int id)
        {
            var commande = await _commandeService.supprimerCommande(id);
            if (commande == null)
            {
                return NotFound($"Commande n{id} n'existe pas");
            }
            return NoContent();
        }
        
        [HttpPost("ajouterArticle/{idCommande}")]
        public async Task<ActionResult<CommandeResponseDTO>> AjouterArticle(int idCommande, ArticleCommandeRequestDTO articleCommande)
        {
            if (!await _commandeService.commandeExists(idCommande))
            {
                return NotFound($"Commande n{idCommande} n'existe pas");
            }

            try
            {
                var commande = await _commandeService.ajouterArticle(idCommande, articleCommande.IdProduit,
                    articleCommande.Quantite);
                return _mapper.Map<CommandeResponseDTO>(commande);
            }
            catch (HttpRequestException e)
            {
                return NotFound(e.Message);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpPost("retirerArticle/{idCommande}")]
        public async Task<ActionResult<CommandeResponseDTO>> RetirerArticle(int idCommande, ArticleCommandeRequestDTO articleCommande)
        {
            if (!await _commandeService.commandeExists(idCommande))
            {
                return NotFound($"Commande n{idCommande} n'existe pas");
            }

            try{
                var commande = await _commandeService.retirerArticle(idCommande, articleCommande.IdProduit,
                    articleCommande.Quantite);
                return _mapper.Map<CommandeResponseDTO>(commande);
            }
            catch (HttpRequestException e)
            {
                return NotFound(e.Message);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpPost("valider/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> ValiderCommande(int id)
        {
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound($"Commande n{id} n'existe pas");
            }

            try{
                var commande = await _commandeService.validerCommande(id);
                return _mapper.Map<CommandeResponseDTO>(commande);
            }
            catch (HttpRequestException e)
            {
                return NotFound(e.Message);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
            
        }
        
        [HttpPost("annuler/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> AnnulerCommande(int id)
        {
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound($"Commande n{id} n'existe pas");
            }

            try{
                var commande = await _commandeService.annulerCommande(id);
                return _mapper.Map<CommandeResponseDTO>(commande);
            }
            catch (HttpRequestException e)
            {
                return NotFound(e.Message);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("expedier/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> ExpedierCommande(int id)
        {
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound($"Commande n{id} n'existe pas");
            }

            try{
                var commande = await _commandeService.expedierCommande(id);
                return _mapper.Map<CommandeResponseDTO>(commande);
            }
            catch (HttpRequestException e)
            {
                return NotFound(e.Message);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("livrer/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> LivrerCommande(int id)
        {
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound($"Commande n{id} n'existe pas");
            }

            try{
                var commande = await _commandeService.livrerCommande(id);
                return _mapper.Map<CommandeResponseDTO>(commande);
            }
            catch (HttpRequestException e)
            {
                return NotFound(e.Message);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("facturer/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> FacturerCommande(int id)
        {
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound($"Commande n{id} n'existe pas");
            }

            try{
                var commande = await _commandeService.facturerCommande(id);
                return _mapper.Map<CommandeResponseDTO>(commande);
            }
            catch (HttpRequestException e)
            {
                return NotFound(e.Message);
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
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
