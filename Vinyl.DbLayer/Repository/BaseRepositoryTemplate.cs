using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public abstract class BaseRepositoryTemplate<T> : BaseRepository
        where T: class
    {
        internal BaseRepositoryTemplate(VinylShopContext context, DbSet<T> set, ILogger<VinylShopContext> logger)
            :base(context, logger)
        {
            Set = set ?? throw new ArgumentNullException(nameof(set));
        }

        protected internal DbSet<T> Set { get; private set; }

        public virtual void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (OnAddValidation(item))
                Set.Add(item);
        }

        public virtual void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (OnUpdateValidation(item))
                Set.Update(item);
        }

        protected virtual bool OnAddValidation(T item)
        {
            return true;
        }

        protected virtual bool OnUpdateValidation(T item)
        {
            return true;
        }

        public virtual void Delete(Guid id)
        {
            var entity = Get(id);
            if (entity == null)
                return;

            Set.Remove(entity);
        }

        public virtual T Get(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return Set.Find(id);
        }
        
        public virtual IQueryable<T> GetAll()
        {
            return Set.AsQueryable();
        }        
    }
}
