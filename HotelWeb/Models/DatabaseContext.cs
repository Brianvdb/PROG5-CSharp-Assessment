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
    }
}