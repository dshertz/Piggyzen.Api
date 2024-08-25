using Microsoft.EntityFrameworkCore;

namespace piggyzen.api.Data
{
    public class PiggyzenContext : DbContext
    {
        public PiggyzenContext(DbContextOptions options) : base(options)
        {
        }
    }
}