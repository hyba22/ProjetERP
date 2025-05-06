using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetERP.ViewModels
{
    public class QuoteViewModel
    {
        public int QuoteId { get; set; }

        [Required(ErrorMessage = "L'ID du client est requis.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Le titre est requis.")]
        [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Le montant est requis.")]
        [Range(0, double.MaxValue, ErrorMessage = "Le montant doit être positif.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "La date d'émission est requise.")]
        public DateTime IssueDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required(ErrorMessage = "Le statut est requis.")]
        [StringLength(50, ErrorMessage = "Le statut ne peut pas dépasser 50 caractères.")]
        public string Status { get; set; }

        public string Details { get; set; }
    }
}