using GoldenRaspberryAwards.Core.Model;
using GoldenRaspberryAwards.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwards.Infrastructure.Data
{
    public class GoldenRaspberryAwardsContext : DbContext
    {
        public GoldenRaspberryAwardsContext(DbContextOptions<GoldenRaspberryAwardsContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<MovieProducer> MovieProducers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MovieMapping());
            modelBuilder.ApplyConfiguration(new ProducerMapping());
            modelBuilder.ApplyConfiguration(new MovieProducerMapping());
        }
    }
}