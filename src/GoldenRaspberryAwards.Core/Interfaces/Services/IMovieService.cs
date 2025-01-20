namespace GoldenRaspberryAwards.Core.Interfaces.Services;

public interface IMovieService
{
    Task<object> GetProducersPrizeIntervals();
}
