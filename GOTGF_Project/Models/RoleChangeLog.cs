using Microsoft.AspNetCore.Mvc;

namespace GOTGF_Project.Models
{
    public class RoleChangeLog
    {
        public int Id { get; set; }
        public string TargetUserId { get; set; } = default!;
        public string ChangedByUserId { get; set; } = default!;
        public string OldRoles { get; set; } = default!;
        public string NewRole { get; set; } = default!;
        public DateTime ChangedAt { get; set; }
    }
}
