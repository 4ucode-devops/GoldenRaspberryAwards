using AutoMapper;
using GoldenRaspberryAwards.Core.Interfaces;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Repositories;
using GoldenRaspberryAwards.Core.Model;
using GoldenRaspberryAwards.CsvDataLoader.Services;
using NSubstitute;

namespace GoldenRaspberryAwards.CsvDataLoader.Tests.Services;

public class CsvProcessorServiceTests
{
    private readonly CsvProcessorService _service;
    private readonly IMovieRepository _movieRepository;
    private readonly IEntityValidator<Movie> _validator;
    private readonly INotifier _notifier;
    private readonly IMapper _mapper;

    public CsvProcessorServiceTests()
    {
        _movieRepository = Substitute.For<IMovieRepository>();
        _validator = Substitute.For<IEntityValidator<Movie>>();
        _notifier = Substitute.For<INotifier>();
        _mapper = Substitute.For<IMapper>();
        _service = new CsvProcessorService(_movieRepository, _validator, _notifier, _mapper);
    }

    [Fact]
    public async Task ProcessCsvAsync_ShouldReturnEmptyList_WhenFilePathIsNullOrEmpty()
    {
        var result = await _service.ProcessCsvAsync("");
        Assert.Empty(result);
        _notifier.Received().Handle("File path is required.");
    }

    [Fact]
    public async Task ProcessCsvAsync_ShouldReturnEmptyList_WhenFileDoesNotExist()
    {
        var result = await _service.ProcessCsvAsync("invalid_path.csv");
        Assert.Empty(result);
        _notifier.Received().Handle("File not found at path: invalid_path.csv");
    }

    [Fact]
    public async Task ProcessCsvAsync_ShouldReturnMovies_WhenValidFileIsProcessed()
    {
        var filePath = "test.csv";
        var csvContent = "Year,Title,Studios,Producer,IsWinner\n2000,Test Movie,Test Studio,Test Producer,yes";
        await File.WriteAllTextAsync(filePath, csvContent);

        Assert.True(File.Exists(filePath), "File was not created successfully.");

        var movieCsv = new MovieCsv
        {
            Year = 2000,
            Title = "Test Movie",
            Studios = "Test Studio",
            Producer = "Test Producer",
            IsWinner = "yes"
        };
        var movie = new Movie
        {
            Year = 2000,
            Title = "Test Movie",
            Studios = "Test Studio",
            IsWinner = "yes"
        };

        _mapper.Map<Movie>(Arg.Any<MovieCsv>()).Returns(movie);
        _validator.Validate(movie).Returns(new List<string>());

        _movieRepository.GetByTitleAndYearAsync(movie.Title, movie.Year).Returns(Task.FromResult<Movie>(null));

        var result = await _service.ProcessCsvAsync(filePath);

        Assert.Single(result);
        Assert.Equal(movie.Title, result[0].Title);

        await _movieRepository.Received().Add(movie);
        await _movieRepository.Received().SaveChangesAsync();

        File.Delete(filePath);
    }

    [Fact]
    public async Task ProcessCsvAsync_ShouldNotAddDuplicateMovies()
    {
        var filePath = "test.csv";
        var csvContent = "Title,Year,IsWinner\nTest Movie,2000,yes";
        await File.WriteAllTextAsync(filePath, csvContent);

        var movie = new Movie { Title = "Test Movie", Year = 2000, IsWinner = "yes" };
        _mapper.Map<Movie>(Arg.Any<MovieCsv>()).Returns(movie);
        _validator.Validate(movie).Returns(new List<string>());
        _movieRepository.GetByTitleAndYearAsync(movie.Title, movie.Year).Returns(Task.FromResult(movie));

        var result = await _service.ProcessCsvAsync(filePath);

        Assert.Empty(result);
        await _movieRepository.DidNotReceive().Add(Arg.Any<Movie>());
        await _movieRepository.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task ProcessCsvAsync_ShouldHandleValidationErrors()
    {
        var filePath = "invalid_data.csv";
        var csvContent = "Title,Year,IsWinner\nTest Movie,2000,invalid_value";
        await File.WriteAllTextAsync(filePath, csvContent);

        var movieCsv = new MovieCsv { Title = "Test Movie", Year = 2000, IsWinner = "invalid_value" };
        var movie = new Movie { Title = "Test Movie", Year = 2000, IsWinner = "invalid_value" };

        _mapper.Map<Movie>(Arg.Any<MovieCsv>()).Returns(movie);

        var validationErrors = new List<string> { "IsWinner must be 'yes' or 'no'." };
        _validator.Validate(movie).Returns(validationErrors);

        var result = await _service.ProcessCsvAsync(filePath);

        Assert.Empty(result);

        _notifier.Received().Handle(Arg.Is<string>(msg => msg.StartsWith("An error occurred while reading the CSV file: Header with name")));
    }

    [Fact]
    public async Task ProcessCsvAsync_ShouldHandleCsvReadingException()
    {
        var filePath = "test.csv";
        await File.WriteAllTextAsync(filePath, "Invalid,CSV,Data");

        var result = await _service.ProcessCsvAsync(filePath);

        Assert.Empty(result);
        _notifier.Received().Handle(Arg.Is<string>(msg => msg.StartsWith("An error occurred while reading the CSV file")));
    }
}
