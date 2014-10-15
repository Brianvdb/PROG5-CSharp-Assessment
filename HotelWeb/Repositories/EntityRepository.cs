using DomainModel.Repositories;
using HotelWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWeb.Repositories
{
    public class EntityRepository<T> : IRepository<T> where T : class
    {
        protected DatabaseContext database;

        public EntityRepository(DatabaseContext database)
        {
            this.database = database;
        }

        List<T> IRepository<T>.GetAll()
        {
            return database.Set<T>().ToList();
        }

        T IRepository<T>.Add(T t)
        {
            database.Set<T>().Add(t);
            database.SaveChanges();
            return t;
        }

        T IRepository<T>.Delete(T t)
        {
            database.Set<T>().Remove(t);
            database.SaveChanges();
            return t;
        }

    }
}