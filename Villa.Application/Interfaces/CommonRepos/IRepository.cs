using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Villla.Domain.Entities;

namespace Villla.Application.Interfaces.CommonRepos
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(
           Expression<Func<T, bool>>? filter = null,
           Func<IQueryable<T>, IQueryable<T>>? include = null,
           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
       );

        T Get(
           Expression<Func<T, bool>> filter,
           Func<IQueryable<T>, IQueryable<T>>? include = null,
           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
       );

        void Create(T entity);
        void Delete(T entity);
        
    }
}
