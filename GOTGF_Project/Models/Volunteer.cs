using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTGF_Project.Models
{
    public class Volunteer
    {
        [Key]
        public int VolunteerID { get; set; } // Primary key
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }   // FK to ApplicationUser
        public virtual ApplicationUser? User { get; set; }

        public string Skills { get; set; }
        public string Availability { get; set; }

        // Navigation
        public virtual ICollection<VolunteerAssignment>? VolunteerAssignments { get; set; } = new List<VolunteerAssignment>();
    }
}