using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetERP.Areas.Identity.Data
{
    public class User : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "varchar(100)")]
        public string? FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "varchar(100)")]
        public string? LastName { get; set; }

        [PersonalData]
        [Column(TypeName = "varchar(50)")]
        public RoleType? Role { get; set; }
    }

    public enum RoleType
    {
        Admin,
        Client,
        Supplier,
        Accountant
    }
}