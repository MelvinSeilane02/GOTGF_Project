using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTGF_Project.Models
{
    [Table("Volunteer_Assignment")]
    public class VolunteerAssignment
    {
        [Key]
        public int AssignmentID { get; set; } // Primary key
        [Required]
        [ForeignKey("Volunteer")]
        public int VolunteerID { get; set; }  // FK
        public virtual Volunteer? Volunteer { get; set; }

        [Required]
        [ForeignKey("Project")]
        public int ProjectID { get; set; }   // FK
        public virtual Project? Project { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}