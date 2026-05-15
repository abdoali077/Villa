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
      
        Task UpdateVillaAsync(Villa entity);
       
    }
}
