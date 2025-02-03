using FluentAssertions;
using GoldenRaspberryAwards.Core.Model;

namespace GoldenRaspberryAwards.Core.Tests.Model;

public class MovieProducerTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var movie = new Movie { Title = "Test Movie" };
        var producer = new Producer { Name = "Test Producer" };
        var movieProducer = new MovieProducer
        {
            Movie = movie,
            Producer = producer
        };

        movieProducer.Movie.Should().Be(movie);
        movieProducer.Producer.Should().Be(producer);
    }
}
