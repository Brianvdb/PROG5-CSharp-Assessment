using DomainModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HotelWeb.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Guest> Guests { get; set; }

        public DbSet<HotelRoom> HotelRooms { get; set; }

        public DbSet<HotelRoomPrice> HotelRoomPrices { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<UserAccount> Accounts { get; set; }


    }
}