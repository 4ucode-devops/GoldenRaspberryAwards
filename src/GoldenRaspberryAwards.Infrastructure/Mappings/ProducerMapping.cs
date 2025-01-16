using GoldenRaspberryAwards.Core.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwards.Infrastructure.Mappings
{
    public class ProducerMapping : IEntityTypeConfiguration<Producer>
    {
        public void Configure(EntityTypeBuilder<Producer> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(255);

            builder.HasMany(p => p.MovieProducers)
                   .WithOne(mp => mp.Producer)
                   .HasForeignKey(mp => mp.ProducerId);
        }
    }
}
