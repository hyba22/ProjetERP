using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ProjetERP.Models;
using ProjetERP.Repositories;
using System.Threading.Tasks;

namespace ProjetERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _repository;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(IClientRepository repository, IEmailSender emailSender, ILogger<ClientsController> logger)
        {
            _repository = repository;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpPost]

        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for client creation.");
                return BadRequest(ModelState);
            }

            var createdClient = await _repository.CreateClient(client);
            _logger.LogInformation($"Client {createdClient.ClientId} created.");

            if (!string.IsNullOrEmpty(client.ContactEmail))
            {
                await _emailSender.SendEmailAsync(client.ContactEmail, "Bienvenue chez ERP APP",
                    "Vous avez été ajouté comme client. Merci de votre confiance.");
            }

            return CreatedAtAction(nameof(GetClient), new { id = createdClient.ClientId }, createdClient);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient(int id)
        {
            var client = await _repository.GetClientById(id);
            if (client == null)
            {
                _logger.LogWarning($"Client {id} not found.");
                return NotFound();
            }
            return Ok(client);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            if (id != client.ClientId)
            {
                _logger.LogWarning($"Client ID mismatch: {id} vs {client.ClientId}.");
                return BadRequest();
            }

            var existingClient = await _repository.GetClientById(id);
            if (existingClient == null)
            {
                _logger.LogWarning($"Client {id} not found for update.");
                return NotFound();
            }

            await _repository.UpdateClient(client);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _repository.GetClientById(id);
            if (client == null)
            {
                _logger.LogWarning($"Client {id} not found for deletion.");
                return NotFound();
            }

            await _repository.DeleteClient(id);
            return NoContent();
        }

        [HttpPost("{clientId}/communications")]

        public async Task<IActionResult> CreateCommunication(int clientId, [FromBody] ClientCommunication communication)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid modelmon model state for communication creation.");
                return BadRequest(ModelState);
            }

            communication.ClientId = clientId;
            var createdCommunication = await _repository.CreateCommunication(communication);

            var client = await _repository.GetClientById(clientId);
            if (!string.IsNullOrEmpty(client.ContactEmail) && communication.Type == "Email")
            {
                await _emailSender.SendEmailAsync(client.ContactEmail, "Mise à jour de communication",
                    communication.Content);
            }

            return CreatedAtAction(nameof(GetClient), new { id = clientId }, createdCommunication);
        }

        [HttpPost("{clientId}/deliveries")]

        public async Task<IActionResult> CreateDelivery(int clientId, [FromBody] Delivery delivery)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for delivery creation.");
                return BadRequest(ModelState);
            }

            delivery.ClientId = clientId;
            var createdDelivery = await _repository.CreateDelivery(delivery);

            var client = await _repository.GetClientById(clientId);
            if (!string.IsNullOrEmpty(client.ContactEmail))
            {
                await _emailSender.SendEmailAsync(client.ContactEmail, "Nouvelle livraison",
                    $"Une nouvelle livraison (Commande: {delivery.OrderNumber}) a été planifiée pour le {delivery.DeliveryDate:dd/MM/yyyy}.");
            }

            return CreatedAtAction(nameof(GetClient), new { id = clientId }, createdDelivery);
        }

        [HttpPost("{clientId}/quotes")]

        public async Task<IActionResult> CreateQuote(int clientId, [FromBody] Quote quote)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for quote creation.");
                return BadRequest(ModelState);
            }

            quote.ClientId = clientId;
            var createdQuote = await _repository.CreateQuote(quote);

            var client = await _repository.GetClientById(clientId);
            if (!string.IsNullOrEmpty(client.ContactEmail))
            {
                await _emailSender.SendEmailAsync(client.ContactEmail, "Nouveau devis",
                    $"Un nouveau devis intitulé '{quote.Title}' a été créé pour un montant de {quote.Amount:C}.");
            }

            return CreatedAtAction(nameof(GetClient), new { id = clientId }, createdQuote);
        }
    }
}