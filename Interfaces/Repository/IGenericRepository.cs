using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetByFilter(Func<T, bool> expression);
        T GetById(object id);
        void Insert(T obj, bool isAvoidSave = false);
        void Update(T obj, bool isAvoidSave = false);
        void Delete(T obj);
        void Save();
        void DeleteAll();
    }
}
