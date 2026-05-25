using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scientific.Entities.Models;

namespace Scientific.Services
{
    public interface IJournalsMinhPvService : IGenericService<JournalsMinhPv>
    {
        Task<JournalsMinhPv> GetAllasyn(int id);
    }
}
