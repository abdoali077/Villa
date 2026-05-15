using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Infrastructure.Data;

namespace Villla.Infrastructure.RepositoryImplementation
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _db;
        internal DbSet<T> dbset;

        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
            dbset = _db.Set<T>();
        }

        // ================= CREATE =================
        public async Task CreateAsync(T entity)
        {
            await dbset.AddAsync(entity);
        }

        // ================= DELETE =================
        public void Delete(T entity)
        {
            dbset.Remove(entity);
        }

        // ================= GET SINGLE =================
        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = dbset;

            query = query.Where(filter);

            if (include != null)
                query = include(query);

            if (orderBy != null)
                query = orderBy(query);

            return await query.FirstOrDefaultAsync();
        }

        // ================= GET ALL =================
        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = dbset;

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include(query);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }
    }
}