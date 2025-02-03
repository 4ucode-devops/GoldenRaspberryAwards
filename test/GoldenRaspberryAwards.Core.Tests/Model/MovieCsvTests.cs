using FluentAssertions;
using GoldenRaspberryAwards.Core.Model;

namespace GoldenRaspberryAwards.Core.Tests.Model;

public class MovieCsvTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var movieCsv = new MovieCsv
        {
            Year = 2022,
            Title = "Test Movie",
            Studios = "Test Studio",
            Producer = "Test Producer",
            IsWinner = "No"
        };

        movieCsv.Year.Should().Be(2022);
        movieCsv.Title.Should().Be("Test Movie");
        movieCsv.Studios.Should().Be("Test Studio");
        movieCsv.Producer.Should().Be("Test Producer");
        movieCsv.IsWinner.Should().Be("No");
    }
}
