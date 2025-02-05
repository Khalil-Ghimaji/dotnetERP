using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.DTO.Facturation;
using Persistence.entities.Commande;
using Persistence.entities.Facturation;

namespace AppGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN,COMPTABLE")]
    public class FacturationController : ControllerBase
    {
        private readonly HttpClient _gestionFacturesClient;
        private readonly HttpClient _gestionCommandesClient;
        private readonly string _facturationUrl = "http://localhost:5105/api/Facturation/";
        private readonly string _gestionCommandesUrl = "http://localhost:5012/api/Commandes/";

        public FacturationController(IHttpClientFactory httpClientFactory)
        {
            _gestionFacturesClient = httpClientFactory.CreateClient("GestionFacturesClient");
            _gestionCommandesClient = httpClientFactory.CreateClient("GestionCommandesClient");
        }

        [HttpGet("factures")]
        public async Task<IActionResult> ListerFactures()
        {
            var response = await _gestionFacturesClient.GetAsync($"{_facturationUrl}");
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpGet("factures/{id}")]
        public async Task<IActionResult> ConsulterFacture(int id)
        {
            var response = await _gestionFacturesClient.GetAsync($"{_facturationUrl}{id}");
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpPost("factures/{factureId}/annuler")]
        public async Task<IActionResult> AnnulerFacture(int factureId)
        {
            try
            {
                var updateFactureDTO = new UpdateFactureDTO
                {
                    StatusFacture = StatusFacture.Annulée
                };

                var json = JsonSerializer.Serialize(updateFactureDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _gestionFacturesClient.PutAsync($"{_facturationUrl}{factureId}", content);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("La facture a été annulée avec succès.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'annulation de la facture : {ex.Message}");
            }
        }

        [HttpPost("factures")]
        public async Task<IActionResult> CreerFacture([FromBody] CreerFactureDTO creerFactureDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var json = JsonSerializer.Serialize(creerFactureDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _gestionFacturesClient.PostAsync($"{_facturationUrl}", content);

            if (response.IsSuccessStatusCode)
            {
                var factureContent = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(factureContent);
                var root = document.RootElement;
                int factureId = root.GetProperty("factureId").GetInt32();

                await VerifierEtMettreAJourStatutCommande(factureId);
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPut("factures/{id}")]
        public async Task<IActionResult> ModifierFacture(int id, [FromBody] UpdateFactureDTO updateFactureDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var json = JsonSerializer.Serialize(updateFactureDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _gestionFacturesClient.PutAsync($"{_facturationUrl}{id}", content);

            if (response.IsSuccessStatusCode)
            {
                await VerifierEtMettreAJourStatutCommande(id);
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }


        [HttpDelete("factures/{id}")]
        public async Task<IActionResult> SupprimerFacture(int id)
        {
            var response = await _gestionFacturesClient.DeleteAsync($"{_facturationUrl}{id}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPost("factures/{factureId}/echeance")]
        public async Task<IActionResult> AjouterEcheance(int factureId, [FromBody] CreerEcheanceDTO creerEcheanceDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var json = JsonSerializer.Serialize(creerEcheanceDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response =
                    await _gestionFacturesClient.PostAsync($"{_facturationUrl}{factureId}/echeance", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }

                try
                {
                    await VerifierEtMettreAJourStatutCommande(factureId);
                }
                catch (Exception)
                {
                    throw new Exception(
                        "Erreur lors de la vérification et de la mise à jour du statut de la commande.");
                }

                return Ok("Échéance ajoutée et statut de la commande vérifié (erreur ignorée si présente).");
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Erreur lors de l'ajout de l'échéance ou la mise à jour du statut : {ex.Message}");
            }
        }


        [HttpGet("factures/{factureId}/echeances")]
        public async Task<IActionResult> ConsulterEcheances(int factureId)
        {
            var response = await _gestionFacturesClient.GetAsync($"{_facturationUrl}{factureId}/echeances");
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpGet("echeances/{echeanceId}")]
        public async Task<IActionResult> ConsulterEcheance(int echeanceId)
        {
            var response = await _gestionFacturesClient.GetAsync($"{_facturationUrl}echeance/{echeanceId}");
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpPut("echeances/{echeanceId}")]
        public async Task<IActionResult> ModifierEcheance(int echeanceId,
            [FromBody] UpdateEcheanceDTO updateEcheanceDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response1 = await _gestionFacturesClient.GetAsync($"{_facturationUrl}echeance/{echeanceId}");
            if (!response1.IsSuccessStatusCode)
            {
                return StatusCode((int)response1.StatusCode, await response1.Content.ReadAsStringAsync());
            }

            var content1 = await response1.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content1);
            var root = jsonDoc.RootElement;
            int factureId = root.GetProperty("factureId").GetInt32();
            /*int factureId = JsonSerializer.Deserialize<Facture>(content1).FactureId;*/
            var json = JsonSerializer.Serialize(updateEcheanceDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _gestionFacturesClient.PutAsync($"{_facturationUrl}echeance/{echeanceId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorContent);
            }

            try
            {
                await VerifierEtMettreAJourStatutCommande(factureId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("L'échéance a été modifiée et le statut de la commande a été vérifié.");
        }

        [HttpPost("echeances/{echeanceId}/payer")]
        public async Task<IActionResult> PayerEcheance(int echeanceId)
        {
            var response1 = await _gestionFacturesClient.GetAsync($"{_facturationUrl}echeance/{echeanceId}");
            if (!response1.IsSuccessStatusCode)
            {
                return StatusCode((int)response1.StatusCode, await response1.Content.ReadAsStringAsync());
            }

            var content1 = await response1.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content1);
            var root = jsonDoc.RootElement;
            int factureId = root.GetProperty("factureId").GetInt32();

            var updateEcheanceDTO = new UpdateEcheanceDTO()
            {
                StatutEcheance = StatutEcheance.Payée
            };
            var json = JsonSerializer.Serialize(updateEcheanceDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _gestionFacturesClient.PutAsync($"{_facturationUrl}echeance/{echeanceId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorContent);
            }

            try
            {
                await VerifierEtMettreAJourStatutCommande(factureId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("L'échéance a été modifiée et le statut de la commande a été vérifié.");
        }

        [HttpDelete("echeances/{echeanceId}")]
        public async Task<IActionResult> SupprimerEcheance(int echeanceId)
        {
            var response1 = await _gestionFacturesClient.GetAsync($"{_facturationUrl}echeance/{echeanceId}");
            var content1 = await response1.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content1);
            var root = jsonDoc.RootElement;
            int factureId = root.GetProperty("factureId").GetInt32();
            var response = await _gestionFacturesClient.DeleteAsync($"{_facturationUrl}echeance/{echeanceId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorContent);
            }

            try
            {
                await VerifierEtMettreAJourStatutCommande(factureId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("L'échéance a été supprimée et le statut de la commande a été vérifié.");
        }

        [HttpGet("factures/{factureId}/pdf")]
        public async Task<IActionResult> GenererFacturePdf(int factureId)
        {
            var response = await _gestionFacturesClient.GetAsync($"{_facturationUrl}{factureId}/pdf");
            var content = await response.Content.ReadAsByteArrayAsync();
            return File(content, "application/pdf", $"Facture_{factureId}.pdf");
        }

        [HttpPost("factures/{factureId}/envoyer-email")]
        public async Task<IActionResult> EnvoyerFactureParEmail(int factureId)
        {
            string verifStatutFactureUrlValidee = $"{_facturationUrl}{factureId}/est_validée";
            var checkValideeResponse = await _gestionFacturesClient.GetAsync(verifStatutFactureUrlValidee);

            if (!checkValideeResponse.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Échec lors de la vérification du statut de la facture (validée) : {checkValideeResponse.ReasonPhrase}");
            }

            var checkValideeContent = await checkValideeResponse.Content.ReadAsStringAsync();
            bool isValidee = bool.Parse(checkValideeContent);

            if (isValidee)
            {
                var response =
                    await _gestionFacturesClient.PostAsync($"{_facturationUrl}{factureId}/envoyer-email", null);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Facture envoyée par e-mail avec succès.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            else
            {
                return BadRequest("La facture doit être validée avant d'être envoyée par e-mail.");
            }
        }

        private async Task VerifierEtMettreAJourStatutCommande(int factureId)
        {
            try
            {
                var response = await _gestionFacturesClient.GetAsync($"{_facturationUrl}{factureId}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erreur lors de la récupération de la facture : {response.ReasonPhrase}");
                }

                var factureContent = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(factureContent);
                var root = document.RootElement;
                var commandeId = root.GetProperty("commandeId").GetInt32();
                string getCommandeUrl = $"{_gestionCommandesUrl}{commandeId}";
                var commandeResponse = await _gestionCommandesClient.GetAsync(getCommandeUrl);
                if (!commandeResponse.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"Erreur lors de la récupération de la commande : {commandeResponse.ReasonPhrase}");
                }

                var commandeContent = await commandeResponse.Content.ReadAsStringAsync();
                using var commandeDocument = JsonDocument.Parse(commandeContent);
                var commandeRoot = commandeDocument.RootElement;
                string statutCommande = commandeRoot.GetProperty("status").GetString();

                string verifStatutFactureUrlPayee = $"{_facturationUrl}{factureId}/est_payée";
                string verifStatutFactureUrlValidee = $"{_facturationUrl}{factureId}/est_validée";
                string rollbackUrl = $"{_gestionCommandesUrl}rollback/{commandeId}";

                var checkPaymentResponse = await _gestionFacturesClient.GetAsync(verifStatutFactureUrlPayee);
                if (!checkPaymentResponse.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"Échec lors de la vérification du statut de la facture (payée) : {checkPaymentResponse.ReasonPhrase}");
                }

                var checkPaymentContent = await checkPaymentResponse.Content.ReadAsStringAsync();
                bool isPayee = bool.Parse(checkPaymentContent);

                var checkValideeResponse = await _gestionFacturesClient.GetAsync(verifStatutFactureUrlValidee);
                if (!checkValideeResponse.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"Échec lors de la vérification du statut de la facture (validée) : {checkValideeResponse.ReasonPhrase}");
                }

                var checkValideeContent = await checkValideeResponse.Content.ReadAsStringAsync();
                bool isValidee = bool.Parse(checkValideeContent);

                string lastStatus;

                if (isPayee)
                {
                    lastStatus = "PAYEE";
                }
                else if (isValidee)
                {
                    if (statutCommande == "FACTUREE" || statutCommande == "EXPEDIEE" || statutCommande == "RESERVEE")
                    {
                        lastStatus = statutCommande;
                    }
                    else if (statutCommande == "PAYEE")
                    {
                        lastStatus = "EXPEDIEE";
                    }
                    else
                    {
                        lastStatus = "FACTUREE";
                    }
                }
                else
                {
                    if (statutCommande == "PAYEE" || statutCommande == "EXPEDIEE")
                    {
                        lastStatus = "EXPEDIEE";
                    }
                    else if (statutCommande == "VALIDEE" || statutCommande == "FACTUREE" ||
                             statutCommande == "RESERVEE")
                    {
                        lastStatus = "VALIDEE";
                    }
                    else
                    {
                        lastStatus = statutCommande;
                    }
                }

                var rollbackContent = new StringContent(
                    JsonSerializer.Serialize(new { lastStatus = lastStatus }),
                    Encoding.UTF8, "application/json"
                );

                var rollbackResponse = await _gestionCommandesClient.PostAsync(rollbackUrl, rollbackContent);
                if (!rollbackResponse.IsSuccessStatusCode)
                {
                    var errorContent = await rollbackResponse.Content.ReadAsStringAsync();
                    throw new Exception($"Échec lors du rollback du statut de la commande : {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur dans la mise à jour du statut de la commande : {ex.Message}");
            }
        }
    }
}