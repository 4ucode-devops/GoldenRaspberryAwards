using FluentAssertions;
using GoldenRaspberryAwards.Core.Model;

namespace GoldenRaspberryAwards.Core.Tests.Model;

public class ProducerTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var producer = new Producer { Name = "Test Producer" };

        producer.Name.Should().Be("Test Producer");
        producer.MovieProducers.Should().BeEmpty();
    }
}
