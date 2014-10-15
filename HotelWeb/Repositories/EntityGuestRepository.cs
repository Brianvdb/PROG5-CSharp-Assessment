using DomainModel;
using HotelWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWeb.Repositories
{
    public class EntityGuestRepository : EntityRepository<Guest>
    {
        public EntityGuestRepository(DatabaseContext database) : base(database)
        {

        }
    }
}