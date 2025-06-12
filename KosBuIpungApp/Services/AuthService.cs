// ===== Services/AuthService.cs =====
using KosBuIpungApp.Models;
using System.Linq;

namespace KosBuIpungApp.Services
{
    public static class AuthService
    {
        public static User CurrentUser { get; private set; }

        public static bool Login(string username, string password)
        {
            var user = DataService.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower() && u.Password == password);
            if (user != null)
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}