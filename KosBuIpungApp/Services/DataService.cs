// ===== Services/DataService.cs =====
using KosBuIpungApp.Models;
using KosBuIpungApp.Enums;
using System.Collections.Generic;
using System;

namespace KosBuIpungApp.Services
{
    public static class DataService
    {
        public static List<User> Users { get; set; }
        public static List<RoomType> RoomTypes { get; set; }
        public static List<Room> Rooms { get; set; }
        public static List<Tenant> Tenants { get; set; }
        public static List<Billing> Billings { get; set; }

        public static void InitializeData()
        {
            Users = new List<User>
            {
                new User { UserId = 1, Username = "admin", Password = "123", FullName = "Bu Ipung", PhoneNumber = "081234567890", Role = UserRole.Admin },
                new User { UserId = 2, Username = "budi", Password = "123", FullName = "Budi Santoso", PhoneNumber = "081111111111", Role = UserRole.User },
                new User { UserId = 3, Username = "siti", Password = "123", FullName = "Siti Aminah", PhoneNumber = "082222222222", Role = UserRole.User },
                new User { UserId = 4, Username = "tono", Password = "123", FullName = "Tono Stark", PhoneNumber = "083333333333", Role = UserRole.User }
            };

            RoomTypes = new List<RoomType>
            {
                new RoomType { RoomTypeId = 1, TypeName = "Standard", Price = 750000, Facilities = "Kasur, Lemari, Meja" },
                new RoomType { RoomTypeId = 2, TypeName = "AC", Price = 1200000, Facilities = "Kasur, Lemari, Meja, AC" },
                new RoomType { RoomTypeId = 3, TypeName = "VIP", Price = 1800000, Facilities = "Kasur, Lemari, Meja, AC, KM Dalam" }
            };

            Rooms = new List<Room>
            {
                new Room { RoomId = 1, RoomNumber = "101", RoomTypeId = 1, Status = RoomStatus.Terisi },
                new Room { RoomId = 2, RoomNumber = "102", RoomTypeId = 1, Status = RoomStatus.Tersedia },
                new Room { RoomId = 3, RoomNumber = "201", RoomTypeId = 2, Status = RoomStatus.Terisi },
                new Room { RoomId = 4, RoomNumber = "202", RoomTypeId = 2, Status = RoomStatus.Tersedia },
                new Room { RoomId = 5, RoomNumber = "301", RoomTypeId = 3, Status = RoomStatus.Tersedia }
            };

            Tenants = new List<Tenant>
            {
                new Tenant { TenantId = 1, UserId = 2, RoomId = 1, CheckInDate = new DateTime(2024, 1, 15) }, // Budi di kamar 101
                new Tenant { TenantId = 2, UserId = 3, RoomId = 3, CheckInDate = new DateTime(2024, 2, 1) } // Siti di kamar 201
            };

            Billings = new List<Billing>
            {
                new Billing { BillingId = 1, TenantId = 1, Amount = 750000, DueDate = new DateTime(2025, 6, 15), Status = BillingStatus.BelumLunas },
                new Billing { BillingId = 2, TenantId = 2, Amount = 1200000, DueDate = new DateTime(2025, 6, 1), Status = BillingStatus.Lunas },
                new Billing { BillingId = 3, TenantId = 2, Amount = 1200000, DueDate = new DateTime(2025, 7, 1), Status = BillingStatus.MenungguVerifikasi, ProofOfPaymentPath = "C:/bukti/siti_juli.jpg" }
            };
        }
    }
}