using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Repositories
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();

        T Add(T t);

        T Delete(T t);
    }
}
