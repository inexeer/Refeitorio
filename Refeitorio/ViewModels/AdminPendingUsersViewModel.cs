using Refeitorio.Models;

namespace Refeitorio.ViewModels
{
    public class AdminPendingUsersViewModel
    {
        public List<User> PendingUsers { get; set; } = new();
    }
}
