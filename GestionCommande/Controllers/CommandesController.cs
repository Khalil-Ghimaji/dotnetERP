using System.Text.Json;
using AutoMapper;
using GestionCommande.DTOs;
using GestionCommande.Services;
using Microsoft.AspNetCore.Mvc;
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
            
            if (!await _commandeService.commandeExists(id))
            {
                return NotFound($"Commande n{id} n'existe pas");
            }

            try
            {
                var commande =
                    await _commandeService.modifierCommande(id, commandeDto.ClientId, commandeDto.dateCommande);
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
            catch (Exception _)
            {
                return BadRequest("Erreur lors de la modification de la commande");
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
            catch(BadHttpRequestException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception _)
            {
                return BadRequest("Erreur lors de la création de la commande");
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
            catch (Exception _)
            {
                return BadRequest("Erreur lors de l'ajout de l'article");
            }
        }
        
        [HttpPost("retirerArticle/{idCommande}")]
        public async Task<ActionResult<CommandeResponseDTO>> RetirerArticle(int idCommande, ArticleCommandeRequestDTO articleCommande)
        {
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
            catch (Exception _)
            {
                return BadRequest("Erreur lors du retrait de l'article");
            }
        }
        
        [HttpPost("valider/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> ValiderCommande(int id)
        {
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
            catch (Exception _)
            {
                return BadRequest("Erreur lors de la validation");
            }
            
        }
        
        [HttpDelete("annuler/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> AnnulerCommande(int id)
        {
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
            catch (Exception _)
            {
                return BadRequest("Erreur lors de l'annulation");
            }
        }
        
        [HttpPost("reserver/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> ReserverCommande(int id)
        {
            try
            {
                var commande = await _commandeService.reserverCommande(id);
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
            catch (Exception _)
            {
                return BadRequest("Erreur lors de la réservation");
            }
        }

        [HttpPost("expedier/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> ExpedierCommande(int id)
        {
            try
            {
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
            catch (Exception _)
            {
                return BadRequest("Erreur lors de l'expédition");
            }
        }

        [HttpPost("payer/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> PayerCommande(int id)
        {
            try
            {
                var commande = await _commandeService.payerCommande(id);
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
            catch (Exception _)
            {
                return BadRequest("Erreur lors du paiement");
            }
        }

        [HttpPost("facturer/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> FacturerCommande(int id)
        {
            try
            {
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
            catch (Exception _)
            {
                return BadRequest("Erreur lors de la facturation");
            }
        }

        [HttpPost("rollback/{id}")]
        public async Task<ActionResult<CommandeResponseDTO>> RollbackCommande(int id, dynamic body)
        {
            try
            {
                var statusCommande = Enum.Parse<StatusCommande>(JsonSerializer.Deserialize<JsonElement>(body)
                    .GetProperty("lastStatus").GetString());
                var commande = await _commandeService.rollback(id, statusCommande);
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
            catch (Exception _)
            {
                return BadRequest("Erreur lors du rollback");
            }
        }
        

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
