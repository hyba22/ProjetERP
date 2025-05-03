using System.ComponentModel.DataAnnotations;

namespace ProjetERP.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [EmailAddress]
        [StringLength(100)]
        public string ContactEmail { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}