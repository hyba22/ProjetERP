using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetERP.Models
{
    public class TermsAndConditions
    {
        public int TermsId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public string Content { get; set; }

        public int Version { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Supplier Supplier { get; set; }
    }
}