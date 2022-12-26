using Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DataLayer.Repository
{
    public class GenericRepository<T> : IGenericRepository<T>, IDisposable where T : class
    {
        private DbSet<T> _entities;
        private string _errorMessage = string.Empty;
        private bool _isDisposed;
        public GenericRepository(IUnitOfWork unitOfWork)
        : this(unitOfWork.Context)
        {
        }
        public GenericRepository(DbContext context)
        {
            _isDisposed = false;
            Context = context;
        }
        public DbContext Context { get; set; }
        public virtual IQueryable<T> Table
        {
            get { return Entities; }
        }
        protected virtual DbSet<T> Entities
        {
            get { return _entities ?? (_entities = Context.Set<T>()); }
        }
        public void Dispose()
        {
            if (Context != null)
                Context.Dispose();
            _isDisposed = true;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return Entities.ToList();
        }
        public virtual T GetById(object id)
        {
            return Entities.Find(id);
        }
        public virtual void Insert(T entity, bool isAvoidSave = false)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                Entities.Add(entity);
                if (!isAvoidSave) Context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public void BulkInsert(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                {
                    throw new ArgumentNullException("entities");
                }
                Context.Set<T>().AddRange(entities);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public virtual void Update(T entity, bool isAvoidSave)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                SetEntryModified(entity);
                if(!isAvoidSave) Context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }
        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Entities.Remove(entity);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }
        public virtual void SetEntryModified(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public IEnumerable<T> GetByFilter(Func<T, bool> expression)
        {
            return Entities.Where(expression);
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        public void DeleteAll()
        {
            foreach (var item in GetAll())
            {
                Entities.Remove(item);
            }
            this.Save();
        }
    }
}
