using System.ComponentModel.DataAnnotations;

namespace ProjetERP.ViewModels
{
    public class ClientViewModel
    {
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Le nom est requis.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "L'email est requis.")]
        [StringLength(100, ErrorMessage = "L'email ne peut pas dépasser 100 caractères.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string ContactEmail { get; set; }

        [Required(ErrorMessage = "Le téléphone est requis.")]
        [StringLength(20, ErrorMessage = "Le téléphone ne peut pas dépasser 20 caractères.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "L'adresse est requise.")]
        public string Address { get; set; }
    }
}