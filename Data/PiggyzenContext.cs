using Microsoft.EntityFrameworkCore;

namespace piggyzen.api.Data
{
    public class PiggyzenContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public PiggyzenContext(DbContextOptions options) : base(options)
        {
        }
    }
}