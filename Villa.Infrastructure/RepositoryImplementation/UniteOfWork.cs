using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Infrastructure.Data;

namespace Villla.Infrastructure.RepositoryImplementation
{
    public class UniteOfWork : IUnitOfWork
    {
        public IVillaRepository Villas { get; private set; }
        public IVillaNumberRepository VillaNumbers { get; private set; }
        public IAmenityRepository Amenities { get; private set; }
        public IBookingRepository Bookings { get; private set; }
        public IApplicationUserRepository ApplicationUsers { get; private set; }
        private readonly ApplicationDbContext _db;
        public UniteOfWork(ApplicationDbContext db)
        {
            _db = db;
            Villas = new VillaRepository(_db);
            VillaNumbers = new VillaNumberRepository(_db);  
            Amenities = new AmenityRepository(_db);
            Bookings = new BookingRepository(_db);
            ApplicationUsers = new ApplicationUserRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
