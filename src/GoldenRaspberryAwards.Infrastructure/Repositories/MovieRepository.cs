using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Repositories;
using GoldenRaspberryAwards.Core.Model;
using GoldenRaspberryAwards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwards.Infrastructure.Repositories;

public class MovieRepository : Repository<Movie>, IMovieRepository
{
    public MovieRepository(GoldenRaspberryAwardsContext dataContext, INotifier notifier)
        : base(dataContext, notifier) { }

    public async Task<Movie> GetByTitleAndYearAsync(string title, int year)
    {
        return await _dbSet
            .Include(m => m.MovieProducers)
            .FirstOrDefaultAsync(m =>
                m.Title.Equals(title, StringComparison.OrdinalIgnoreCase)
                && m.Year == year);
    }

    public override async Task<IEnumerable<Movie>> GetAll()
    {
        return await _dbSet
            .Include(m => m.MovieProducers)
            .ThenInclude(mp => mp.Producer)
            .ToListAsync();
    }
}
