using Asp.Versioning;
using GameStore.API.Controllers;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Services;
using GoldenRaspberryAwards.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace GoldenRaspberryAwards.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/csv-processor")]
public class CsvProcessorController : MainController
{
    private readonly ICsvProcessorService<Movie> _csvProcessorService;

    public CsvProcessorController(INotifier notifier, ICsvProcessorService<Movie> csvProcessorService) : base(notifier)
    {
        _csvProcessorService = csvProcessorService;
    }

    [HttpPost("csv")]
    public async Task<ActionResult> ProcessCsv([FromForm] IFormFile file)
    {
        if (file is null)
        {
            NotifyError("File is required.");
            return CustomResponse();
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FileName);

        if (System.IO.File.Exists(filePath))
        {
            NotifyError("File already exists.");
            return CustomResponse();
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var movies = await _csvProcessorService.ProcessCsvAsync(filePath);

        if (movies.Count == 0)
        {
            NotifyError("No movies found.");
            return CustomResponse();
        }

        return CustomResponse(movies);
    }
}
