using FluentAssertions;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Model;
using GoldenRaspberryAwards.Infrastructure.Data;
using GoldenRaspberryAwards.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace GoldenRaspberryAwards.Infrastructure.Tests.Repositories;

public class MovieRepositoryTests
{
    private readonly GoldenRaspberryAwardsContext _dbContext;
    private readonly INotifier _notifier;
    private readonly DbSet<Movie> _dbSet;
    private readonly MovieRepository _repository;

    public MovieRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<GoldenRaspberryAwardsContext>()
            .UseInMemoryDatabase("GoldenRaspberryAwardsTestDb")
            .Options;

        _dbContext = new GoldenRaspberryAwardsContext(options);
        _notifier = Substitute.For<INotifier>();
        _repository = new MovieRepository(_dbContext, _notifier);

        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetByTitleAndYearAsync_ShouldReturnMovie_WhenExists()
    {
        var movie = new Movie { Title = "Test Movie", Year = 2022, Studios = "Test Studio" };

        _dbContext.Movies.Add(movie);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetByTitleAndYearAsync("Test Movie", 2022);

        result.Should().NotBeNull();
        result.Title.Should().Be("Test Movie");
        result.Year.Should().Be(2022);
    }

    [Fact]
    public async Task GetByTitleAndYearAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _repository.GetByTitleAndYearAsync("Nonexistent Movie", 2023);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_ShouldReturnMovies()
    {
        _dbContext.Movies.RemoveRange(_dbContext.Movies);
        await _dbContext.SaveChangesAsync();

        _dbContext.Movies.AddRange(new Movie { Title = "Movie 1", Year = 2020, Studios = "Studio A" }, new Movie { Title = "Movie 2", Year = 2021, Studios = "Studio B" });
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetAll();

        var uniqueMovies = result.GroupBy(m => new { m.Title, m.Year })
                                 .Select(g => g.First())
                                 .ToList();

        uniqueMovies.Should().HaveCount(2);

        uniqueMovies.Should().Contain(m => m.Title == "Movie 1" && m.Year == 2020);
        uniqueMovies.Should().Contain(m => m.Title == "Movie 2" && m.Year == 2021);
    }
}
