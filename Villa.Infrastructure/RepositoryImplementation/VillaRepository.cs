using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Domain.Entities;
using Villla.Infrastructure.Data;

namespace Villla.Infrastructure.RepositoryImplementation
{
    public class VillaRepository : GenericRepository<Villa>, IVillaRepository
    {
       
        public VillaRepository(ApplicationDbContext db) : base(db)   
        {
            //_db = db;
        }

        #region Before Generic Repository
        //public void CreateVilla(Villa entity)
        //{
        //    _db.Villas.Add(entity);
        //}

        //public void DeleteVilla(Villa entity)
        //{
        //    _db.Villas.Remove(entity);
        //}

        //public Villa Get(Expression<Func<Villa, bool>> filter, Func<IQueryable<Villa>, IQueryable<Villa>>? include = null, Func<IQueryable<Villa>, IOrderedQueryable<Villa>>? orderBy = null)
        //{
        //    IQueryable<Villa> query = _db.Set<Villa>();
        //    query.Where(filter);
        //    if (include != null)
        //    {
        //        query = include(query);
        //    }
        //    if (orderBy != null)
        //    {
        //        query = orderBy(query);
        //    }
        //    return query.FirstOrDefault(filter);

        //}

        //public IEnumerable<Villa> GetAll(Expression<Func<Villa, bool>>? filter = null, Func<IQueryable<Villa>, IQueryable<Villa>>? include = null, Func<IQueryable<Villa>, IOrderedQueryable<Villa>>? orderBy = null)
        //{
        //   IQueryable<Villa>query= _db.Set<Villa>();
        //    if(filter != null)
        //    {
        //        query = query.Where(filter);
        //    }
        //    if(include != null)
        //    {
        //        query = include(query);
        //    }
        //    if(orderBy != null)
        //    {
        //        query = orderBy(query);
        //    }
        //    return query;
        //}
        #endregion
        

        public void UpdateVilla(Villa entity)
        {
            _db.Villas.Update(entity);
           
        }
    }
}
