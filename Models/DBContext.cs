using Microsoft.EntityFrameworkCore;

namespace WebAppDBMS.Models
{
    public partial class DBContext : DbContext

    {

        public DBContext(DbContextOptions<DBContext> options)
                    : base(options)
        {
            Database.EnsureCreated();
        }
    }
}