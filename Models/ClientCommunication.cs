using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetERP.Models
{
    public class ClientCommunication
    {
        public int CommunicationId { get; set; }

        [Required(ErrorMessage = "L'ID du client est requis.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Le type de communication est requis.")]
        [StringLength(50, ErrorMessage = "Le type ne peut pas dépasser 50 caractères.")]
        public string Type { get; set; } // Email, Phone, Meeting

        [Required(ErrorMessage = "Le contenu est requis.")]
        public string Content { get; set; }

        public DateTime CommunicationDate { get; set; } = DateTime.Now;

        public Client Client { get; set; }
    }
}