using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetERP.ViewModels
{
    public class DeliveryViewModel
    {
        public int DeliveryId { get; set; }

        [Required(ErrorMessage = "L'ID du client est requis.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Le numéro de commande est requis.")]
        [StringLength(100, ErrorMessage = "Le numéro de commande ne peut pas dépasser 100 caractères.")]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "La date de livraison est requise.")]
        public DateTime DeliveryDate { get; set; }

        [Required(ErrorMessage = "Le statut est requis.")]
        [StringLength(50, ErrorMessage = "Le statut ne peut pas dépasser 50 caractères.")]
        public string Status { get; set; }

        public string Details { get; set; }
    }
}