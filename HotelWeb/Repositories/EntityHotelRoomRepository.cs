using DomainModel;
using HotelWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWeb.Repositories
{
    public class EntityHotelRoomRepository : EntityRepository<HotelRoom>
    {
        public EntityHotelRoomRepository(DatabaseContext database) : base(database)
        {

        }
    }
}