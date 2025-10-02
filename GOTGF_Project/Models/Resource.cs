using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTGF_Project.Models
{
    public class Resource
    {
        [Key]
        public int ResourceID { get; set; } // Primary key
        [Required]
        [ForeignKey("Project")]
        public int ProjectID { get; set; }  // FK
        public virtual Project? Project { get; set; }

        [Required]
        [StringLength(100)]
        public string ResourceName { get; set; }
        [Required]
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}