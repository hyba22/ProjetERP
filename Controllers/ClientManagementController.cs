using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetERP.Models;
using ProjetERP.Repositories;
using ProjetERP.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjetERP.Controllers
{
    [Authorize]
    public class ClientManagementController : Controller
    {
        private readonly IClientRepository _repository;

        public ClientManagementController(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            // Ajout de logs pour déboguer l'authentification et les rôles
            var userName = User.Identity.Name;
            var isAuthenticated = User.Identity.IsAuthenticated;
            var userRoles = User.IsInRole("Admin") ? "Admin" : User.IsInRole("Client") ? "Client" : "No relevant role";
            Console.WriteLine($"User: {userName}, IsAuthenticated: {isAuthenticated}, Roles: {userRoles}");

            var clients = await _repository.GetAllClients() ?? Enumerable.Empty<Client>();
            var viewModels = clients.Select(c => new ClientViewModel
            {
                ClientId = c.ClientId,
                Name = c.Name,
                ContactEmail = c.ContactEmail,
                Phone = c.Phone,
                Address = c.Address
            }).ToList();

            return View(viewModels);
        }

        [Authorize(Roles = "Admin,Client")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                return View(viewModel);
            }

            var client = new Client
            {
                Name = viewModel.Name,
                ContactEmail = viewModel.ContactEmail,
                Phone = viewModel.Phone,
                Address = viewModel.Address
            };

            await _repository.CreateClient(client);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _repository.GetClientById(id);
            if (client == null) return NotFound();

            var viewModel = new ClientViewModel
            {
                ClientId = client.ClientId,
                Name = client.Name,
                ContactEmail = client.ContactEmail,
                Phone = client.Phone,
                Address = client.Address
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientViewModel viewModel)
        {
            if (id != viewModel.ClientId) return BadRequest();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                return View(viewModel);
            }

            var client = await _repository.GetClientById(id);
            if (client == null) return NotFound();

            client.Name = viewModel.Name;
            client.ContactEmail = viewModel.ContactEmail;
            client.Phone = viewModel.Phone;
            client.Address = viewModel.Address;
            client.UpdatedAt = DateTime.Now;

            await _repository.UpdateClient(client);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _repository.GetClientById(id);
            if (client == null) return NotFound();

            var viewModel = new ClientViewModel
            {
                ClientId = client.ClientId,
                Name = client.Name,
                ContactEmail = client.ContactEmail,
                Phone = client.Phone,
                Address = client.Address
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteClient(id);
            return RedirectToAction(nameof(Index));
        }

        // ---------------- Communications ----------------

        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> Communications(int id)
        {
            var communications = await _repository.GetCommunicationsByClientId(id) ?? Enumerable.Empty<ClientCommunication>();
            var client = await _repository.GetClientById(id);
            if (client == null) return NotFound();

            var viewModels = communications.Select(c => new ClientCommunicationViewModel
            {
                CommunicationId = c.CommunicationId,
                ClientId = c.ClientId,
                Type = c.Type,
                Content = c.Content,
                CommunicationDate = c.CommunicationDate
            }).ToList();

            ViewData["ClientName"] = client.Name;
            ViewData["ClientId"] = client.ClientId;
            return View(viewModels);
        }

        [Authorize(Roles = "Admin,Client")]
        public IActionResult CreateCommunication(int id)
        {
            var model = new ClientCommunicationViewModel
            {
                ClientId = id,
                CommunicationDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCommunication(ClientCommunicationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                return View(viewModel);
            }

            var communication = new ClientCommunication
            {
                ClientId = viewModel.ClientId,
                Type = viewModel.Type,
                Content = viewModel.Content,
                CommunicationDate = viewModel.CommunicationDate
            };

            try
            {
                await _repository.CreateCommunication(communication);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Erreur : {ex.Message}";
                if (ex.InnerException != null) errorMessage += $"\nInner: {ex.InnerException.Message}";
                ModelState.AddModelError(string.Empty, errorMessage);
                Console.WriteLine(errorMessage);
                return View(viewModel);
            }

            return RedirectToAction(nameof(Communications), new { id = viewModel.ClientId });
        }

        // ---------------- Deliveries ----------------

        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> Deliveries(int id)
        {
            var deliveries = await _repository.GetDeliveriesByClientId(id) ?? Enumerable.Empty<Delivery>();
            var client = await _repository.GetClientById(id);
            if (client == null) return NotFound();

            var viewModels = deliveries.Select(d => new DeliveryViewModel
            {
                DeliveryId = d.DeliveryId,
                ClientId = d.ClientId,
                OrderNumber = d.OrderNumber,
                DeliveryDate = d.DeliveryDate,
                Status = d.Status,
                Details = d.Details
            }).ToList();

            ViewData["ClientName"] = client.Name;
            ViewData["ClientId"] = client.ClientId;
            return View(viewModels);
        }

        [Authorize(Roles = "Admin,Client")]
        public IActionResult CreateDelivery(int id)
        {
            var model = new DeliveryViewModel
            {
                ClientId = id,
                DeliveryDate = DateTime.Now,
                Status = "Pending"
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDelivery(DeliveryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                return View(viewModel);
            }

            var delivery = new Delivery
            {
                ClientId = viewModel.ClientId,
                OrderNumber = viewModel.OrderNumber,
                DeliveryDate = viewModel.DeliveryDate,
                Status = viewModel.Status,
                Details = viewModel.Details
            };

            try
            {
                await _repository.CreateDelivery(delivery);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Erreur : {ex.Message}";
                if (ex.InnerException != null) errorMessage += $"\nInner: {ex.InnerException.Message}";
                ModelState.AddModelError(string.Empty, errorMessage);
                Console.WriteLine(errorMessage);
                return View(viewModel);
            }

            return RedirectToAction(nameof(Deliveries), new { id = viewModel.ClientId });
        }

        // ---------------- Quotes ----------------

        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> Quotes(int id)
        {
            var quotes = await _repository.GetQuotesByClientId(id) ?? Enumerable.Empty<Quote>();
            var client = await _repository.GetClientById(id);
            if (client == null) return NotFound();

            var viewModels = quotes.Select(q => new QuoteViewModel
            {
                QuoteId = q.QuoteId,
                ClientId = q.ClientId,
                Title = q.Title,
                Amount = q.Amount,
                IssueDate = q.IssueDate,
                ExpiryDate = q.ExpiryDate,
                Status = q.Status,
                Details = q.Details
            }).ToList();

            ViewData["ClientName"] = client.Name;
            ViewData["ClientId"] = client.ClientId;
            return View(viewModels);
        }

        [Authorize(Roles = "Admin,Client")]
        public IActionResult CreateQuote(int id)
        {
            var model = new QuoteViewModel
            {
                ClientId = id,
                IssueDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddMonths(1),
                Status = "Pending"
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuote(QuoteViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                return View(viewModel);
            }

            var client = await _repository.GetClientById(viewModel.ClientId);
            if (client == null)
            {
                ModelState.AddModelError(string.Empty, "Le client spécifié n'existe pas.");
                return View(viewModel);
            }

            var quote = new Quote
            {
                ClientId = viewModel.ClientId,
                Title = viewModel.Title,
                Amount = viewModel.Amount,
                IssueDate = viewModel.IssueDate,
                ExpiryDate = viewModel.ExpiryDate,
                Status = viewModel.Status,
                Details = viewModel.Details
            };

            try
            {
                await _repository.CreateQuote(quote);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Erreur : {ex.Message}";
                if (ex.InnerException != null) errorMessage += $"\nInner: {ex.InnerException.Message}";
                ModelState.AddModelError(string.Empty, errorMessage);
                Console.WriteLine(errorMessage);
                return View(viewModel);
            }

            return RedirectToAction(nameof(Quotes), new { id = viewModel.ClientId });
        }
    }
}