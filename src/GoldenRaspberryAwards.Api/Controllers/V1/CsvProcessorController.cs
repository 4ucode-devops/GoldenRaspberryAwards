using Asp.Versioning;
using GameStore.API.Controllers;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Services;
using GoldenRaspberryAwards.Core.Model;
using GoldenRaspberryAwards.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace GoldenRaspberryAwards.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/csv-processor")]
public class CsvProcessorController : MainController
{
    private readonly ICsvProcessorService<Movie> _csvProcessorService;
    private readonly GoldenRaspberryAwardsContext _dbContext;

    public CsvProcessorController(
        INotifier notifier,
        ICsvProcessorService<Movie> csvProcessorService,
        GoldenRaspberryAwardsContext dbContext) : base(notifier)
    {
        _csvProcessorService = csvProcessorService;
        _dbContext = dbContext;
    }

    [HttpPost("csv")]
    public async Task<IActionResult> ProcessCsv([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            NotifyError("Arquivo CSV é obrigatório.");
            return CustomResponse();
        }

        try
        {
            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var movies = await _csvProcessorService.ProcessCsvAsync(filePath);

            return CustomResponse(movies);
        }
        catch (Exception ex)
        {
            NotifyError(ex.Message);
            return CustomResponse();
        }
    }

    public async Task<List<Movie>> ProcessCsvAsync(string filePath)
    {
        var movies = new List<Movie>();

        try
        {
            using (var reader = new StreamReader(filePath))
            {
                string? line;
                bool isFirstLine = true;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue; // Pula o cabeçalho
                    }

                    var values = line.Split('\t');
                    if (values.Length < 5) throw new Exception($"Linha inválida no arquivo: {line}");

                    var movieTitle = values[1];
                    var movieYear = int.Parse(values[0]);
                    var movieIsWinner = values[4].Equals("yes", StringComparison.OrdinalIgnoreCase) ? "yes" : "no";

                    var existingMovie = await _dbContext.Movies
                        .FirstOrDefaultAsync(m => m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase)
                                                  && m.Year == movieYear);

                    if (existingMovie != null)
                    {
                        continue;
                    }

                    // Criar novo filme
                    var movie = new Movie
                    {
                        Year = movieYear,
                        Title = movieTitle,
                        Studios = values[2],
                        IsWinner = movieIsWinner
                    };

                    var producers = values[3]
                        .Split(',')
                        .Select(p => p.Trim())
                        .ToList();

                    foreach (var producerName in producers)
                    {
                        var producer = await GetOrCreateProducerAsync(producerName);
                        movie.MovieProducers.Add(new MovieProducer { Movie = movie, Producer = producer });
                    }

                    movies.Add(movie);
                }
            }

            // Adiciona os filmes que não existem no banco
            if (movies.Any())
            {
                await _dbContext.Movies.AddRangeAsync(movies);
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao processar o arquivo CSV. Verifique o formato do arquivo.", ex);
        }

        return movies;
    }
    private async Task<Producer> GetOrCreateProducerAsync(string name)
    {
        // Verifica se o produtor já existe
        var existingProducer = await _dbContext.Producers
            .FirstOrDefaultAsync(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (existingProducer != null)
            return existingProducer;

        // Cria um novo produtor
        var newProducer = new Producer { Name = name };
        _dbContext.Producers.Add(newProducer);
        await _dbContext.SaveChangesAsync();

        return newProducer;
    }
}