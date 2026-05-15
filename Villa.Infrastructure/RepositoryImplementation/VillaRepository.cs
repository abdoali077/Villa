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

        public async Task UpdateVillaAsync(Villa entity)
        {
            _db.Villas.Update(entity);
            await Task.CompletedTask;
        }
    }
}
