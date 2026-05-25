using Scientific.Entities.Models;

namespace Scientific.Repositories
{
    public class AuthorRepository : GenericRepository<AuthorsBaoTg>
    {
        public AuthorRepository() : base()
        {
        }

        public AuthorRepository(ScientificJournalTrendDBContext context) : base(context)
        {
        }

        // Viết các phương thức đặc thù cho Author ở đây
    }
}