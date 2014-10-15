using DomainModel;
using HotelWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWeb.Repositories
{
    public class EntityInvoiceRepository : EntityRepository<Invoice>
    {
        public EntityInvoiceRepository(DatabaseContext database) : base(database)
        {

        }
    }
}