using Microsoft.EntityFrameworkCore;

namespace Piggyzen.Api.Data
{
    public class PiggyzenContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionRelation> TransactionRelations { get; set; } // Lagt till TransactionRelation
        public DbSet<CategorizationHistory> CategorizationHistory { get; set; }
        public DbSet<TransactionTag> TransactionTags { get; set; }
        public DbSet<Tag> Tags { get; set; } // Lägg till detta!

        public PiggyzenContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kaskadborttagning för barntransaktioner
            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.ChildTransactions)
                .WithOne(ct => ct.ParentTransaction)
                .HasForeignKey(ct => ct.ParentTransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kaskadborttagning för relaterade transaktioner i TransactionRelation
            modelBuilder.Entity<TransactionRelation>()
                .HasOne(tr => tr.SourceTransaction)
                .WithMany(t => t.SourceRelations)
                .HasForeignKey(tr => tr.SourceTransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransactionRelation>()
                .HasOne(tr => tr.TargetTransaction)
                .WithMany(t => t.TargetRelations)
                .HasForeignKey(tr => tr.TargetTransactionId)
                .OnDelete(DeleteBehavior.NoAction); // Undvik cirkulära borttagningar

            // Konfiguration för TransactionTag
            modelBuilder.Entity<TransactionTag>()
                .HasKey(tt => new { tt.TransactionId, tt.TagId });

            modelBuilder.Entity<TransactionTag>()
                .HasOne(tt => tt.Transaction)
                .WithMany(t => t.TransactionTags)
                .HasForeignKey(tt => tt.TransactionId);

            modelBuilder.Entity<TransactionTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TransactionTags)
                .HasForeignKey(tt => tt.TagId);
        }
    }
}