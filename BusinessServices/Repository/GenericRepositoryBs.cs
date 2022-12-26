using Core;
using Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices.Repository
{
    public class GenericRepositoryBs<T,TEntity> where T : IGenericRepository<TEntity> where TEntity:class
    {
        T _dal;
        public GenericRepositoryBs(T dal)
        {
            _dal = dal;
        }


        public virtual void Delete(TEntity entity)
        {
            _dal.Delete(entity);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dal.GetAll();
        }

        public virtual IEnumerable<TEntity> GetByFilter(Func<TEntity, bool> expression)
        {
            return _dal.GetByFilter(expression);
        }

        public virtual TEntity GetById(object id)
        {
            return _dal.GetById(id);
        }

        public virtual void Insert(TEntity obj, bool isAvoidSave=false)
        {
            _dal.Insert(obj);
        }

        public virtual void Save()
        {
            _dal.Save();
        }

        public virtual void Update(TEntity obj,bool isAvoidSave)
        {
            _dal.Update(obj, isAvoidSave);
        }

        public virtual void DeleteAll()
        {
            _dal.DeleteAll();
        }

    }
}
