namespace GOTGF_Project.Models
{
    // simple user + roles view
    public class UserRoleViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public IEnumerable<string> CurrentRoles { get; set; } = new List<string>();
    }

    // index view model
    public class UsersIndexViewModel
    {
        public List<UserRoleViewModel> Users { get; set; } = new();
        public List<string> AvailableRoles { get; set; } = new();
    }

    // edit specific model
    public class UserEditViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string SelectedRole { get; set; }
        public List<string> AvailableRoles { get; set; } = new();
    }

}
