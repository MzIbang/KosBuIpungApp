// ===== Models/Tenant.cs =====
using System;

namespace KosBuIpungApp.Models
{
    public class Tenant
    {
        public int TenantId { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
    }
}