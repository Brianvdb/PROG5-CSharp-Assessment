using DomainModel;
using HotelWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWeb.Repositories
{
    public class EntityAddressRepository : EntityRepository<Address>
    {
        public EntityAddressRepository(DatabaseContext database) : base(database)
        {

        }
    }
}