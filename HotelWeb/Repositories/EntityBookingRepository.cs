using DomainModel;
using HotelWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWeb.Repositories
{
    public class EntityBookingRepository : EntityRepository<Booking>
    {
        public EntityBookingRepository(DatabaseContext database) : base(database)
        {

        }
    }
}