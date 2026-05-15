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
    public class AmenityRepository : GenericRepository<Amenity>, IAmenityRepository
    {
        public AmenityRepository(ApplicationDbContext db) : base(db) { }
        public async Task UpdateAmenityAsync(Amenity entity)
        {
            _db.Amenities.Update(entity);
                
            await Task.CompletedTask;


        }
    }
}
