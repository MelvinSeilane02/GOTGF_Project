using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTGF_Project.Models
{
    public class Report
    {
        [Key]
        public int ReportID { get; set; } // Primary key
        public string ReportType { get; set; }
        public DateTime ReportDate { get; set; } = DateTime.UtcNow;


        // Foreign keys
        [Required]
        [ForeignKey("Project")]
        public int ProjectID { get; set; }
        public virtual Project? Project { get; set; }

        [Required]
        [ForeignKey("User")]
        public string GeneratedBy { get; set; }  // User ID
        public virtual ApplicationUser? User { get; set; }
    }
}