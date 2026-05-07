using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Villla.Domain.Entities;

namespace Villla.Application.Interfaces.CommonRepos
{
    public interface IVillaRepository : IRepository<Villa>
    {
        //IEnumerable<Villa> GetAll(
        //    Expression<Func<Villa, bool>>? filter = null,
        //    Func<IQueryable<Villa>, IQueryable<Villa>>? include = null,
        //    Func<IQueryable<Villa>, IOrderedQueryable<Villa>>? orderBy = null
        //);

        // Villa Get(
        //    Expression<Func<Villa, bool>> filter,
        //    Func<IQueryable<Villa>, IQueryable<Villa>>? include = null,
        //    Func<IQueryable<Villa>, IOrderedQueryable<Villa>>? orderBy = null
        //);

        //void CreateVilla(Villa entity);
        void UpdateVilla(Villa entity);
        //void DeleteVilla(Villa entity);
       
    }
}
