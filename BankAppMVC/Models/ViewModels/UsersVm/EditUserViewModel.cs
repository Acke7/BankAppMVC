namespace BankAppMVC.Models.ViewModels.UsersVm
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }

        public List<string> SelectedRoles { get; set; } = new();
        public List<string> AllRoles { get; set; } = new();
    }
}
