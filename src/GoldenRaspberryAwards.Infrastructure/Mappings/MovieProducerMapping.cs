using GoldenRaspberryAwards.Core.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwards.Infrastructure.Mappings
{
    public class MovieProducerMapping : IEntityTypeConfiguration<MovieProducer>
    {
        public void Configure(EntityTypeBuilder<MovieProducer> builder)
        {
            builder.HasKey(mp => new { mp.MovieId, mp.ProducerId });

            builder.HasOne(mp => mp.Movie)
                   .WithMany(m => m.MovieProducers)
                   .HasForeignKey(mp => mp.MovieId);

            builder.HasOne(mp => mp.Producer)
                   .WithMany(p => p.MovieProducers)
                   .HasForeignKey(mp => mp.ProducerId);
        }
    }
}
