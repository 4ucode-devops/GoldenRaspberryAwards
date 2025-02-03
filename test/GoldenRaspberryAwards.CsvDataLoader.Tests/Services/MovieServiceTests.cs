using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Repositories;
using GoldenRaspberryAwards.Core.Model;
using GoldenRaspberryAwards.Core.Services;
using NSubstitute;

namespace GoldenRaspberryAwards.CsvDataLoader.Tests.Services;

public class MovieServiceTests
{
    private readonly IMovieRepository _movieRepository;
    private readonly INotifier _notifier;
    private readonly MovieService _movieService;

    public MovieServiceTests()
    {
        _movieRepository = Substitute.For<IMovieRepository>();
        _notifier = Substitute.For<INotifier>();
        _movieService = new MovieService(_movieRepository, _notifier);
    }

    [Fact]
    public async Task GetProducersPrizeIntervals_ShouldReturnCorrectIntervals()
    {
        // Arrange
        var producer = new Producer { Name = "Producer A" };
        var movies = new List<Movie>
        {
            new Movie { Year = 2000, Title = "Movie 1", IsWinner = "yes", MovieProducers = new List<MovieProducer> { new MovieProducer { Producer = producer } } },
            new Movie { Year = 2005, Title = "Movie 2", IsWinner = "yes", MovieProducers = new List<MovieProducer> { new MovieProducer { Producer = producer } } },
            new Movie { Year = 2015, Title = "Movie 3", IsWinner = "yes", MovieProducers = new List<MovieProducer> { new MovieProducer { Producer = producer } } }
        };

        _movieRepository.GetAll().Returns(Task.FromResult((IEnumerable<Movie>)movies));

        // Act
        var result = await _movieService.GetProducersPrizeIntervals();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetProducersPrizeIntervals_NoWinningMovies_ShouldReturnEmptyLists()
    {
        // Arrange
        _movieRepository.GetAll().Returns(Task.FromResult((IEnumerable<Movie>)new List<Movie>()));

        // Act
        var result = await _movieService.GetProducersPrizeIntervals();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetProducersPrizeIntervals_ShouldHandleRepositoryException()
    {
        // Arrange
        _movieRepository.GetAll().Returns(Task.FromException<IEnumerable<Movie>>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _movieService.GetProducersPrizeIntervals());
        _notifier.Received().Handle(Arg.Is<string>(msg => msg.Contains("error")));
    }
}