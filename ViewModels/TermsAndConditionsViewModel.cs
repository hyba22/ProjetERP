using System.ComponentModel.DataAnnotations;

namespace ProjetERP.ViewModels
{
    public class TermsAndConditionsViewModel
    {
        public int TermsId { get; set; }
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Le contenu est requis.")]
        public string Content { get; set; }

        public int Version { get; set; }
    }
}