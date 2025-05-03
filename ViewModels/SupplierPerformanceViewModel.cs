using System.ComponentModel.DataAnnotations;

namespace ProjetERP.ViewModels
{
    public class SupplierPerformanceViewModel
    {
        public int PerformanceId { get; set; }
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "La date d'évaluation est requise.")]
        public DateTime EvaluationDate { get; set; }

        [Required(ErrorMessage = "Le score est requis.")]
        public int Score { get; set; }

        public string Comments { get; set; }
    }
}