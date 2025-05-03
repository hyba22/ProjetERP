using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetERP.Models;
using ProjetERP.Repositories;
using ProjetERP.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetERP.Controllers
{
    [Authorize]
    public class SupplierManagementController : Controller
    {
        private readonly ISupplierRepository _repository;

        public SupplierManagementController(ISupplierRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _repository.GetAllSuppliers() ?? Enumerable.Empty<Supplier>();
            var viewModels = suppliers.Select(s => new SupplierViewModel
            {
                SupplierId = s.SupplierId,
                Name = s.Name,
                ContactEmail = s.ContactEmail,
                Phone = s.Phone,
                Address = s.Address
            }).ToList();

            return View(viewModels);
        }

        [Authorize(Roles = "Admin,Supplier")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Supplier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                return View(viewModel);
            }

            var supplier = new Supplier
            {
                Name = viewModel.Name,
                ContactEmail = viewModel.ContactEmail,
                Phone = viewModel.Phone,
                Address = viewModel.Address
            };

            await _repository.CreateSupplier(supplier);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Supplier")]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _repository.GetSupplierById(id);
            if (supplier == null)
            {
                return NotFound();
            }

            var viewModel = new SupplierViewModel
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                ContactEmail = supplier.ContactEmail,
                Phone = supplier.Phone,
                Address = supplier.Address
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Supplier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierViewModel viewModel)
        {
            if (id != viewModel.SupplierId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                return View(viewModel);
            }

            var supplier = await _repository.GetSupplierById(id);
            if (supplier == null)
            {
                return NotFound();
            }

            supplier.Name = viewModel.Name;
            supplier.ContactEmail = viewModel.ContactEmail;
            supplier.Phone = viewModel.Phone;
            supplier.Address = viewModel.Address;
            supplier.UpdatedAt = DateTime.Now;

            await _repository.UpdateSupplier(supplier);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Supplier")]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _repository.GetSupplierById(id);
            if (supplier == null)
            {
                return NotFound();
            }

            var viewModel = new SupplierViewModel
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                ContactEmail = supplier.ContactEmail,
                Phone = supplier.Phone,
                Address = supplier.Address
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Supplier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteSupplier(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Supplier")]
        public async Task<IActionResult> Terms(int id)
        {
            var terms = await _repository.GetTermsBySupplierId(id) ?? Enumerable.Empty<TermsAndConditions>();
            var supplier = await _repository.GetSupplierById(id);
            if (supplier == null)
            {
                return NotFound();
            }

            var viewModels = terms.Select(t => new TermsAndConditionsViewModel
            {
                TermsId = t.TermsId,
                SupplierId = t.SupplierId,
                Content = t.Content,
                Version = t.Version
            }).ToList();

            ViewData["SupplierName"] = supplier.Name;
            ViewData["SupplierId"] = supplier.SupplierId;
            return View(viewModels);
        }

        [Authorize(Roles = "Admin,Supplier")]
        public IActionResult CreateTerms(int id)
        {
            var model = new TermsAndConditionsViewModel { SupplierId = id };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Supplier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTerms(TermsAndConditionsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                return View(viewModel);
            }

            var terms = new TermsAndConditions
            {
                SupplierId = viewModel.SupplierId,
                Content = viewModel.Content,
                Version = viewModel.Version,
                CreatedAt = DateTime.Now
            };

            try
            {
                await _repository.CreateTerms(terms);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erreur lors de la création des conditions générales : {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nInner exception: {ex.InnerException.Message}";
                }
                ModelState.AddModelError(string.Empty, errorMessage);
                Console.WriteLine(errorMessage);
                return View(viewModel);
            }

            return RedirectToAction(nameof(Terms), new { id = viewModel.SupplierId });
        }

        [Authorize(Roles = "Admin,Supplier")]
        public async Task<IActionResult> Performances(int id)
        {
            var performances = await _repository.GetPerformancesBySupplierId(id) ?? Enumerable.Empty<SupplierPerformance>();
            var supplier = await _repository.GetSupplierById(id);
            if (supplier == null)
            {
                return NotFound();
            }

            var viewModels = performances.Select(p => new SupplierPerformanceViewModel
            {
                PerformanceId = p.PerformanceId,
                SupplierId = p.SupplierId,
                EvaluationDate = p.EvaluationDate,
                Score = p.Score,
                Comments = p.Comments
            }).ToList();

            ViewData["SupplierName"] = supplier.Name;
            ViewData["SupplierId"] = supplier.SupplierId;
            return View(viewModels);
        }

        [Authorize(Roles = "Admin,Supplier")]
        public IActionResult CreatePerformance(int id)
        {
            var model = new SupplierPerformanceViewModel { SupplierId = id, EvaluationDate = DateTime.Now };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Supplier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePerformance(SupplierPerformanceViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                return View(viewModel);
            }

            var performance = new SupplierPerformance
            {
                SupplierId = viewModel.SupplierId,
                EvaluationDate = viewModel.EvaluationDate,
                Score = viewModel.Score,
                Comments = viewModel.Comments
            };

            try
            {
                await _repository.CreatePerformance(performance);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erreur lors de la création de la performance : {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nInner exception: {ex.InnerException.Message}";
                }
                ModelState.AddModelError(string.Empty, errorMessage);
                Console.WriteLine(errorMessage);
                return View(viewModel);
            }

            return RedirectToAction(nameof(Performances), new { id = viewModel.SupplierId });
        }

        [Authorize(Roles = "Admin,Supplier")]
        public async Task<IActionResult> Contracts(int id)
        {
            var contracts = await _repository.GetContractsBySupplierId(id) ?? Enumerable.Empty<Contract>();
            var supplier = await _repository.GetSupplierById(id);
            if (supplier == null)
            {
                return NotFound();
            }

            var viewModels = contracts.Select(c => new ContractViewModel
            {
                ContractId = c.ContractId,
                SupplierId = c.SupplierId,
                Title = c.Title,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Terms = c.Terms,
                Status = c.Status
            }).ToList();

            ViewData["SupplierName"] = supplier.Name;
            ViewData["SupplierId"] = supplier.SupplierId;
            return View(viewModels);
        }

        [Authorize(Roles = "Admin,Supplier")]
        public IActionResult CreateContract(int id)
        {
            var model = new ContractViewModel { SupplierId = id, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) };
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Supplier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContract(ContractViewModel viewModel)
        {
            Console.WriteLine("CreateContract action reached - POST request received");
            Console.WriteLine($"ViewModel values - SupplierId: {viewModel.SupplierId}, Title: {viewModel.Title}, StartDate: {viewModel.StartDate}, EndDate: {viewModel.EndDate}, Terms: {viewModel.Terms}, Status: {viewModel.Status}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                return View(viewModel);
            }

            Console.WriteLine("ModelState validation passed");

            var supplier = await _repository.GetSupplierById(viewModel.SupplierId);
            if (supplier == null)
            {
                ModelState.AddModelError(string.Empty, "Le fournisseur spécifié n'existe pas.");
                return View(viewModel);
            }

            var contract = new Contract
            {
                SupplierId = viewModel.SupplierId,
                Title = viewModel.Title,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                Terms = viewModel.Terms,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            try
            {
                await _repository.CreateContract(contract);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erreur lors de la création du contrat : {ex.Message}");
                return View(viewModel);
            }

            return RedirectToAction(nameof(Contracts), new { id = viewModel.SupplierId });
        }


        [Authorize(Roles = "Admin,Supplier")]
        public async Task<IActionResult> TerminateContract(int id, int supplierId)
        {
            var contract = await _repository.GetContractById(id);
            if (contract == null)
            {
                return NotFound();
            }

            contract.Status = "Terminated";
            await _repository.UpdateContract(contract);
            return RedirectToAction(nameof(Contracts), new { id = supplierId });
        }
    }
}