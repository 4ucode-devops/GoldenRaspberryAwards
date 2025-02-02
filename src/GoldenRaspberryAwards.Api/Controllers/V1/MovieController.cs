using Asp.Versioning;
using GoldenRaspberryAwards.Core.Interfaces;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoldenRaspberryAwards.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/movies")]
public class MovieController : MainController
{
    private readonly IMovieService _movieService;

    public MovieController(
        INotifier notifier,
        IAspNetUser user,
        IMovieService movieService) : base(notifier, user)
    {
        _movieService = movieService;
    }

    [HttpGet("producers-prize-intervals")]
    public async Task<IActionResult> GetProducersPrizeIntervals()
    {
        try
        {
            var result = await _movieService.GetProducersPrizeIntervals();
            return CustomResponse(result);
        }
        catch (Exception ex)
        {
            NotifyError(ex.Message);
            return CustomResponse();
        }
    }
}
