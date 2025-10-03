using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GOTGF_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add your custom fields
        [Required]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; }
        [Required]
        [RegularExpression("Volunteer|Donor|Admin", ErrorMessage = "Role must be Volunteer, Donor, or Admin.")]
        public string RoleType { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Donation> Donations { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<DisasterAlert> DisasterAlerts { get; set; }
    }
}