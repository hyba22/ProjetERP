using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetERP.ViewModels
{
    public class ClientCommunicationViewModel
    {
        public int CommunicationId { get; set; }

        [Required(ErrorMessage = "L'ID du client est requis.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Le type de communication est requis.")]
        [StringLength(50, ErrorMessage = "Le type ne peut pas dépasser 50 caractères.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Le contenu est requis.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "La date de communication est requise.")]
        public DateTime CommunicationDate { get; set; }
    }
}
