// ===== Models/User.cs =====
using KosBuIpungApp.Enums;

namespace KosBuIpungApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public UserRole Role { get; set; }
    }
}