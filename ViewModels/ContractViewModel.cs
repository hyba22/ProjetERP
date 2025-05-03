using System.ComponentModel.DataAnnotations;

namespace ProjetERP.ViewModels
{
    public class ContractViewModel
    {
        public int ContractId { get; set; }
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Le titre est requis.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "La date de début est requise.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La date de fin est requise.")]
        public DateTime EndDate { get; set; }

        public string Terms { get; set; }

        public string Status { get; set; } 
    }
}