using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Domain.Entities;
using Villla.Infrastructure.Data;

namespace Villla.Infrastructure.RepositoryImplementation
{
    public class VillaNumberRepository : GenericRepository<VillaNumber>, IVillaNumberRepository
    {
        public VillaNumberRepository(ApplicationDbContext db) : base(db)
        {
        }
        public void UpdateVillaNumber(VillaNumber entity)
        {
            _db.VillaNumbers.Update(entity);
        }
    }
}
