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

        public T Get(int id)
        {
            return database.Set<T>().Find(id);
        }

        public List<T> GetAll()
        {
            return database.Set<T>().ToList();
        }

        public T Add(T t)
        {
            database.Set<T>().Add(t);
            database.SaveChanges();
            return t;
        }

        public T Delete(T t)
        {
            database.Set<T>().Remove(t);
            database.SaveChanges();
            return t;
        }

        public void UpdateDatabase()
        {
            database.SaveChanges();
        }

    }
}