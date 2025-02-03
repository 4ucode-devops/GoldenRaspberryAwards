using FluentAssertions;
using GoldenRaspberryAwards.Core.Model;

namespace GoldenRaspberryAwards.Core.Tests.Model;

public class MovieTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var movie = new Movie
        {
            Year = 2022,
            Title = "Test Movie",
            Studios = "Test Studio",
            IsWinner = "Yes"
        };

        movie.Year.Should().Be(2022);
        movie.Title.Should().Be("Test Movie");
        movie.Studios.Should().Be("Test Studio");
        movie.IsWinner.Should().Be("Yes");
        movie.MovieProducers.Should().BeEmpty();
    }
}
