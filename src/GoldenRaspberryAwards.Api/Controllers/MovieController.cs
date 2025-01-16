using Microsoft.AspNetCore.Mvc;
using GoldenRaspberryAwards.Infrastructure.Data;
using GoldenRaspberryAwards.Core.Model;

namespace GoldenRaspberryAwards.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly GoldenRaspberryAwardsContext _context;

        public MoviesController(GoldenRaspberryAwardsContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Processar o arquivo CSV
            await ProcessCsvAsync(file);

            return Ok("File uploaded and data processed successfully.");
        }

        private async Task ProcessCsvAsync(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var csvProcessor = new CsvProcessor<Movie, CsvMovie>(/* validador e notificador */);
                var movies = await csvProcessor.ProcessCsvAsync(stream);

                if (movies.Any())
                {
                    _context.Movies.AddRange(movies);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

}
