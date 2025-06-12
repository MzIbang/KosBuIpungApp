// ===== Models/Room.cs =====
using KosBuIpungApp.Enums;

namespace KosBuIpungApp.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public int RoomTypeId { get; set; }
        public RoomStatus Status { get; set; }

        // Properti tambahan untuk tampilan di DataGridView
        public string TypeName { get; set; }
        public decimal Price { get; set; }
    }
}