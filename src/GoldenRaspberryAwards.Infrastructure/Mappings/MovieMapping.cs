using GoldenRaspberryAwards.Core.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwards.Infrastructure.Mappings
{
    public class MovieMapping : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Title).IsRequired().HasMaxLength(255);
            builder.Property(m => m.Studios).HasMaxLength(255);
            builder.Property(m => m.Year).IsRequired();
            builder.Property(m => m.IsWinner).HasDefaultValue(false);

            builder.HasMany(m => m.MovieProducers)
                   .WithOne(mp => mp.Movie)
                   .HasForeignKey(mp => mp.MovieId);
        }
    }
}

