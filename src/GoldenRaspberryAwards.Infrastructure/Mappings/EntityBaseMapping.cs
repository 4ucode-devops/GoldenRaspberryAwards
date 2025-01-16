using Microsoft.EntityFrameworkCore;
using GoldenRaspberryAwards.Core.Model;

namespace GoldenRaspberryAwards.Infrastructure.Mappings;

public class EntityBaseMapping<T> : IEntityTypeConfiguration<T> where T : EntityBase
{

}
