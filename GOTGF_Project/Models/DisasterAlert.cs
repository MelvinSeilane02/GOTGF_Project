using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTGF_Project.Models
{
    [Table("Disaster_Alerts")]
    public class DisasterAlert
    {
        [Key]
        public int AlertID { get; set; }   // Primary key
        [Required]
        [ForeignKey("Project")]
        public int ProjectID { get; set; } // FK
        public virtual Project? Project { get; set; }

        [Required]
        [ForeignKey("User")]
        public string CreatedBy { get; set; } // User ID
        public virtual ApplicationUser? User { get; set; }

        [Required]
        public string AlertMessage { get; set; }
        public DateTime AlertDate { get; set; } = DateTime.UtcNow;
    }
}