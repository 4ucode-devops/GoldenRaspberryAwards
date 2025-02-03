using Asp.Versioning;
using AutoMapper;
using GoldenRaspberryAwards.Core.Dtos;
using GoldenRaspberryAwards.Core.Interfaces;
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
    private readonly IMapper _mapper;

    public CsvProcessorController(
        INotifier notifier,
        IAspNetUser user,
        ICsvProcessorService<Movie> csvProcessorService,
        IMapper mapper) : base(notifier, user)
    {
        _csvProcessorService = csvProcessorService;
        _mapper = mapper;
    }

    [HttpPost("csv")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ProcessCsv([FromForm] CsvUploadInput input)
    {
        if (input.File == null || input.File.Length == 0)
        {
            NotifyError("Error: Arquivo CSV é obrigatório.");
            return CustomResponse();
        }

        try
        {
            var filePath = Path.Combine(Path.GetTempPath(), input.File.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await input.File.CopyToAsync(stream);
            }

            var importedMovies = await _csvProcessorService.ProcessCsvAsync(filePath);

            if (!importedMovies.Any())
                return CustomResponse("Nenhum registro importado (ou todos já existiam).");

            var importedMoviesDto = _mapper.Map<List<MovieDTO>>(importedMovies);

            return CustomResponse(importedMoviesDto);
        }
        catch (Exception ex)
        {
            NotifyError(ex.Message);
            return CustomResponse();
        }
    }
}
