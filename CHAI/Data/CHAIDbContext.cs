using CHAI.Models;
using Microsoft.EntityFrameworkCore;

namespace CHAI.Data
{
    /// <summary>
    /// Class representing the <see cref="CHAIDbContext"/>.
    /// </summary>
    public class CHAIDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CHAIDbContext"/> class.
        /// </summary>
        /// <param name="options"><see cref="DbContextOptions"/> for the <see cref="DbContext"/>.</param>
        public CHAIDbContext(DbContextOptions<CHAIDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// Gets or Sets a <see cref="DbSet{Trigger}"/> for storing <see cref="Trigger"/> entities.
        /// </summary>
        public DbSet<Trigger> Triggers { get; set; }

        /// <summary>
        /// Method for Configuring database entities.
        /// </summary>
        /// <param name="modelBuilder">Builder used to define mapping of entities to database tables.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Trigger>()
                .Property(trigger => trigger.BitsCondition)
                .HasConversion<int>();

            modelBuilder.Entity<Trigger>()
                .Property(trigger => trigger.CooldownUnit)
                .HasConversion<int>();
        }
    }
}
