using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Repositories;
using GoldenRaspberryAwards.Core.Interfaces.Services;
using GoldenRaspberryAwards.SharedServices.Services;

namespace GoldenRaspberryAwards.Core.Services;

public class MovieService : BaseService, IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(
        IMovieRepository movieRepository,
        INotifier notifier) : base(notifier)
    {
        _movieRepository = movieRepository;
    }

    public async Task<object> GetProducersPrizeIntervals()
    {
        try
        {
            var movies = await _movieRepository.GetAll();

            var producersPrizeData = movies
                .Where(m => m.IsWinner.Equals("yes", StringComparison.OrdinalIgnoreCase))
                .SelectMany(m => m.MovieProducers, (movie, producer) => new
                {
                    producer = producer.Producer.Name,
                    year = movie.Year
                })
                .GroupBy(p => p.producer)
                .Select(g => new
                {
                    producer = g.Key,
                    wins = g.OrderBy(x => x.year).ToList()
                })
                .ToList();

            var min = new List<object>();
            var max = new List<object>();

            foreach (var producerData in producersPrizeData)
            {
                var wins = producerData.wins;

                for (int i = 1; i < wins.Count; i++)
                {
                    var previousWin = wins[i - 1].year;
                    var followingWin = wins[i].year;
                    var interval = followingWin - previousWin;

                    if (min.Count == 0 || interval < (int)min[0].GetType().GetProperty("interval").GetValue(min[0]))
                    {
                        min.Clear();
                        min.Add(new
                        {
                            producer = producerData.producer,
                            interval,
                            previousWin,
                            followingWin
                        });
                    }

                    if (max.Count == 0 || interval > (int)max[0].GetType().GetProperty("interval").GetValue(max[0]))
                    {
                        max.Clear();
                        max.Add(new
                        {
                            producer = producerData.producer,
                            interval,
                            previousWin,
                            followingWin
                        });
                    }
                }
            }

            return new
            {
                min,
                max
            };
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }
}
