using ProjetERP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjetERP.Repositories
{
    public interface ISupplierRepository
    {
        Task<Supplier> CreateSupplier(Supplier supplier);
        Task<Supplier> GetSupplierById(int id);
        Task<IEnumerable<Supplier>> GetAllSuppliers();
        Task UpdateSupplier(Supplier supplier);
        Task DeleteSupplier(int id);
        Task<Contract> CreateContract(Contract contract);
        Task<Contract> GetContractById(int contractId);
        Task<IEnumerable<Contract>> GetContractsBySupplierId(int supplierId);
        Task UpdateContract(Contract contract);
        Task<TermsAndConditions> CreateTerms(TermsAndConditions terms);
        Task<IEnumerable<TermsAndConditions>> GetTermsBySupplierId(int supplierId);
        Task<SupplierPerformance> CreatePerformance(SupplierPerformance performance);
        Task<IEnumerable<SupplierPerformance>> GetPerformancesBySupplierId(int supplierId);
    }
}