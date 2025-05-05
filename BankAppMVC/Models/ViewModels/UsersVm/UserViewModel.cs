namespace BankAppMVC.Models.ViewModels.UsersVm
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Roles => string.Join(", ", RoleList);
        public IList<string> RoleList { get; set; } = new List<string>();
    }
}
