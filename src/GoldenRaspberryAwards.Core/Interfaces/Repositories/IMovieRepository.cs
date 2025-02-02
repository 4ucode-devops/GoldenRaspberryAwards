using GoldenRaspberryAwards.Core.Model;

namespace GoldenRaspberryAwards.Core.Interfaces.Repositories;

public interface IMovieRepository : IRepository<Movie>
{
    Task<Movie> GetByTitleAndYearAsync(string title, int year);
}
