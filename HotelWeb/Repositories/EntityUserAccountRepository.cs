using DomainModel;
using HotelWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWeb.Repositories
{
    public class EntityUserAccountRepository : EntityRepository<UserAccount>
    {
        public EntityUserAccountRepository(DatabaseContext database)
            : base(database)
        {

        }
    }
}