// ===== Models/RoomType.cs =====
namespace KosBuIpungApp.Models
{
    public class RoomType
    {
        public int RoomTypeId { get; set; }
        public string TypeName { get; set; }
        public decimal Price { get; set; }
        public string Facilities { get; set; }
    }
}