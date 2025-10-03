using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOTGF_Project.Models
{
    public class Donation
    {
        [Key]
        public int DonationID { get; set; } // Primary key
        [Required]
        public decimal Amount { get; set; }
        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        // Foreign keys
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        //Make navigation optional (no [Required] here!)
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [ForeignKey("Project")]
        public int ProjectID { get; set; }
        //Make navigation optional (EF links automatically using ProjectID)
        public virtual Project? Project { get; set; }
    }
}