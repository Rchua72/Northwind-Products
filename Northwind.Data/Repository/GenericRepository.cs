using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Northwind.Data
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> _dbSet { get; set; }
        protected DbContext _context { get; set; }

        public GenericRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("An instance of DbContext is " +
                    "required to use this repository.", "context");
            }

            this._context = context;
            this._dbSet = this._context.Set<T>();
        }

        public virtual IQueryable<T> GetAll()
        {
            return this._dbSet;
        }

        public virtual T GetById(int? id)
        {
            return this._dbSet.Find(id);
        }

        public virtual void Add(T entity)
        {
            DbEntityEntry entry = this._context.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Added;
            }
            else
            {
                this._dbSet.Add(entity);
            }
        }

        public virtual void Update(T entity)
        {
            DbEntityEntry entry = this._context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this._dbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        public virtual void Detach(T entity)
        {
            DbEntityEntry entry = this._context.Entry(entity);

            entry.State = EntityState.Detached;
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry entry = this._context.Entry(entity);

            if (entry.State != EntityState.Deleted)
            {
                entry.State = EntityState.Deleted;
            }
            else
            {
                this._dbSet.Attach(entity);
                this._dbSet.Remove(entity);
            }
        }

        public virtual void Delete(int id)
        {
            var entity = this.GetById(id);

            if (entity != null)
            {
                this.Delete(entity);
            }
        }
    }
}
