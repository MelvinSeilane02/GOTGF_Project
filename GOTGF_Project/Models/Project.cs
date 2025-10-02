using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace GOTGF_Project.Models
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }  // Primary key
        [Required]
        [StringLength(150)]
        public string ProjectName { get; set; }
        public string Description { get; set; }
        [Required]
        [RegularExpression("Planned|Ongoing|Completed", ErrorMessage = "Status must be Planned, Ongoing, or Completed.")]
        public string Status { get; set; }  // Planned, Ongoing, Completed
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // ✅ Navigation properties should be optional and initialized
        public virtual ICollection<Donation>? Donations { get; set; } = new List<Donation>();
        public virtual ICollection<VolunteerAssignment>? VolunteerAssignments { get; set; } = new List<VolunteerAssignment>();
        public virtual ICollection<Resource>? Resources { get; set; } = new List<Resource>();
        public virtual ICollection<Report>? Reports { get; set; } = new List<Report>();
        public virtual ICollection<DisasterAlert>? DisasterAlerts { get; set; } = new List<DisasterAlert>();
    }
}