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
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierRepository _repository;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController(ISupplierRepository repository, IEmailSender emailSender, ILogger<SuppliersController> logger)
        {
            _repository = repository;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Supplier")]
        public async Task<IActionResult> CreateSupplier([FromBody] Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for supplier creation.");
                return BadRequest(ModelState);
            }

            var createdSupplier = await _repository.CreateSupplier(supplier);
            _logger.LogInformation($"Supplier {createdSupplier.SupplierId} created.");

            if (!string.IsNullOrEmpty(supplier.ContactEmail))
            {
                await _emailSender.SendEmailAsync(supplier.ContactEmail, "Bienvenue chez ERP APP",
                    "Vous avez été ajouté comme fournisseur. Voici nos conditions générales.");
            }

            return CreatedAtAction(nameof(GetSupplier), new { id = createdSupplier.SupplierId }, createdSupplier);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplier(int id)
        {
            var supplier = await _repository.GetSupplierById(id);
            if (supplier == null)
            {
                _logger.LogWarning($"Supplier {id} not found.");
                return NotFound();
            }
            return Ok(supplier);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Supplier")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                _logger.LogWarning($"Supplier ID mismatch: {id} vs {supplier.SupplierId}.");
                return BadRequest();
            }

            var existingSupplier = await _repository.GetSupplierById(id);
            if (existingSupplier == null)
            {
                _logger.LogWarning($"Supplier {id} not found for update.");
                return NotFound();
            }

            await _repository.UpdateSupplier(supplier);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _repository.GetSupplierById(id);
            if (supplier == null)
            {
                _logger.LogWarning($"Supplier {id} not found for deletion.");
                return NotFound();
            }

            await _repository.DeleteSupplier(id);
            return NoContent();
        }

        [HttpPost("{supplierId}/contracts")]
        [Authorize(Roles = "Admin,Supplier")]
        public async Task<IActionResult> CreateContract(int supplierId, [FromBody] Contract contract)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for contract creation.");
                return BadRequest(ModelState);
            }

            contract.SupplierId = supplierId;
            var createdContract = await _repository.CreateContract(contract);

            var supplier = await _repository.GetSupplierById(supplierId);
            if (!string.IsNullOrEmpty(supplier.ContactEmail))
            {
                await _emailSender.SendEmailAsync(supplier.ContactEmail, "Nouveau contrat",
                    $"Un nouveau contrat intitulé '{contract.Title}' a été créé.");
            }

            return CreatedAtAction(nameof(GetSupplier), new { id = supplierId }, createdContract);
        }

        [HttpPost("{supplierId}/terms")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTerms(int supplierId, [FromBody] TermsAndConditions terms)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for terms creation.");
                return BadRequest(ModelState);
            }

            terms.SupplierId = supplierId;
            var createdTerms = await _repository.CreateTerms(terms);

            var supplier = await _repository.GetSupplierById(supplierId);
            if (!string.IsNullOrEmpty(supplier.ContactEmail))
            {
                await _emailSender.SendEmailAsync(supplier.ContactEmail, "Nouvelles conditions générales",
                    terms.Content);
            }

            return CreatedAtAction(nameof(GetSupplier), new { id = supplierId }, createdTerms);
        }

        [HttpPost("{supplierId}/performances")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePerformance(int supplierId, [FromBody] SupplierPerformance performance)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for performance creation.");
                return BadRequest(ModelState);
            }

            performance.SupplierId = supplierId;
            var createdPerformance = await _repository.CreatePerformance(performance);

            var supplier = await _repository.GetSupplierById(supplierId);
            if (!string.IsNullOrEmpty(supplier.ContactEmail))
            {
                await _emailSender.SendEmailAsync(supplier.ContactEmail, "Nouvelle évaluation de performance",
                    $"Votre performance a été évaluée avec un score de {performance.Score}. Commentaires : {performance.Comments}");
            }

            return CreatedAtAction(nameof(GetSupplier), new { id = supplierId }, createdPerformance);
        }
    }
}