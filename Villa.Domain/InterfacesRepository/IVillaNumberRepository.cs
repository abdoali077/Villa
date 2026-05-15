using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Domain.Entities;

namespace Villla.Application.Interfaces.CommonRepos
{
    public interface IVillaNumberRepository: IRepository<VillaNumber>
    {
        Task UpdateVillaNumberAsync(VillaNumber entity);
    }
}
