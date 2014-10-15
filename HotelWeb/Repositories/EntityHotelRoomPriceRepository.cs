using DomainModel;
using HotelWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWeb.Repositories
{
    public class EntityHotelRoomPriceRepository : EntityRepository<HotelRoomPrice>
    {
        public EntityHotelRoomPriceRepository(DatabaseContext database) : base(database)
        {

        }
    }
}