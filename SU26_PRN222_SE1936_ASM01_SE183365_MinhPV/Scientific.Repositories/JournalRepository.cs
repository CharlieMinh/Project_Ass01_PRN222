using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scientific.Repositories
{
    public class JournalRepository : GenericRepository<JournalsMinhPv>
    {
        public JournalRepository() : base()
        {
        }

        public JournalRepository(ScientificJournalTrendDBContext context) : base(context)
        {
        }

        // Lấy tất cả Journals và nạp kèm các Papers (bài báo) liên quan
        public async Task<JournalsMinhPv> GetAllasyn(int id)
        {
            return await _context.JournalsMinhPvs
                .Include(x => x.PapersBaoTgs)
                .FirstOrDefaultAsync(x => x.JournalIdMinhPv == id);
        }

        // Lấy danh sách Journals thông qua một Id cụ thể 
        // (Nếu model của bạn có kiểu Id nào đặc thù thì sửa lại nếu cần, ví dụ này dựa theo bài giảng truyền Guid hoặc Id)
        public async Task<List<JournalsMinhPv>> GetByIdAsyn(int id)
        {
            return await _context.JournalsMinhPvs
                .Include(c => c.PapersBaoTgs)
                .Where(x => x.JournalIdMinhPv == id) // Chỉ là ví dụ query
                .ToListAsync();
        }

        // Tìm kiếm các Journals đang Active
        public async Task<List<JournalsMinhPv>> SearchAsync()
        {
            return await _context.JournalsMinhPvs
                .Include(c => c.PapersBaoTgs)
                .Where(c => c.IsActive) // Thay vì Status == 1 như dự án mẫu, model của bạn dùng IsActive (bool)
                .ToListAsync();
        }
    }
}