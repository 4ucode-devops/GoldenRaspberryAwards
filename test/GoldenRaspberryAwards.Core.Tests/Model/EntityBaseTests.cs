using FluentAssertions;
using GoldenRaspberryAwards.Core.Model;

namespace GoldenRaspberryAwards.Core.Tests.Model;

public class EntityBaseTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var entity = new EntityBaseTest();

        entity.Id.Should().NotBeEmpty();
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        entity.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Update_ShouldSetUpdatedAt()
    {
        var entity = new EntityBaseTest();
        entity.Update();

        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ToggleIsDeleted_ShouldToggleStateAndSetUpdatedAt()
    {
        var entity = new EntityBaseTest();
        bool initialState = entity.IsDeleted;

        entity.ToggleIsDeleted();

        entity.IsDeleted.Should().Be(!initialState);
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}

internal class EntityBaseTest : EntityBase { }
