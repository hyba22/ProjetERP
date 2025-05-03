using Microsoft.EntityFrameworkCore;
using ProjetERP.Data;
using ProjetERP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjetERP.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ProjetERPDbContext _context;

        public SupplierRepository(ProjetERPDbContext context)
        {
            _context = context;
        }

        public async Task<Supplier> CreateSupplier(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier> GetSupplierById(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliers()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task UpdateSupplier(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Contract> CreateContract(Contract contract)
        {
            try
            {
                Console.WriteLine($"Attempting to create contract: SupplierId={contract.SupplierId}, Title={contract.Title}");
                _context.Contracts.Add(contract);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Contract created successfully: ContractId={contract.ContractId}");
                return contract;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating contract: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<Contract> GetContractById(int contractId)
        {
            return await _context.Contracts.FindAsync(contractId);
        }

        public async Task<IEnumerable<Contract>> GetContractsBySupplierId(int supplierId)
        {
            return await _context.Contracts
                .Where(c => c.SupplierId == supplierId)
                .ToListAsync();
        }

        public async Task UpdateContract(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }

        public async Task<TermsAndConditions> CreateTerms(TermsAndConditions terms)
        {
            _context.TermsAndConditions.Add(terms);
            await _context.SaveChangesAsync();
            return terms;
        }

        public async Task<IEnumerable<TermsAndConditions>> GetTermsBySupplierId(int supplierId)
        {
            return await _context.TermsAndConditions
                .Where(t => t.SupplierId == supplierId)
                .ToListAsync();
        }

        public async Task<SupplierPerformance> CreatePerformance(SupplierPerformance performance)
        {
            _context.SupplierPerformances.Add(performance);
            await _context.SaveChangesAsync();
            return performance;
        }

        public async Task<IEnumerable<SupplierPerformance>> GetPerformancesBySupplierId(int supplierId)
        {
            return await _context.SupplierPerformances
                .Where(p => p.SupplierId == supplierId)
                .ToListAsync();
        }
    }
}