using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using GoldenRaspberryAwards.Core.Interfaces;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Repositories;
using GoldenRaspberryAwards.Core.Interfaces.Services;
using GoldenRaspberryAwards.Core.Model;
using GoldenRaspberryAwards.SharedServices.Services;
using System.Globalization;

namespace GoldenRaspberryAwards.CsvDataLoader.Services;

public class CsvProcessorService : BaseService, ICsvProcessorService<Movie>
{
    private readonly IMovieRepository _movieRepository;
    private readonly IEntityValidator<Movie> _validator;
    private readonly IMapper _mapper;

    public CsvProcessorService(
        IMovieRepository movieRepository,
        IEntityValidator<Movie> validator,
        INotifier notifier,
        IMapper mapper) : base(notifier)
    {
        _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<Movie>> ProcessCsvAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            _notifier.Handle("File path is required.");
            return new List<Movie>();
        }

        if (!File.Exists(filePath))
        {
            _notifier.Handle($"File not found at path: {filePath}");
            return new List<Movie>();
        }

        var importedMovies = new List<Movie>();

        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            });

            var records = csv.GetRecordsAsync<MovieCsv>();

            await foreach (var record in records)
            {
                var movie = _mapper.Map<Movie>(record);

                var validationErrors = _validator.Validate(movie);
                if (validationErrors.Any())
                {
                    foreach (var error in validationErrors)
                        _notifier.Handle(error);
                    continue;
                }

                var existing = await _movieRepository.GetByTitleAndYearAsync(movie.Title, movie.Year);
                if (existing != null)
                {
                    continue;
                }

                await _movieRepository.Add(movie);
                importedMovies.Add(movie);
            }

            await _movieRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            if (!importedMovies.Any())
            {
                _notifier.Handle($"An error occurred while reading the CSV file: {ex.Message}");
            }
        }

        return importedMovies;
    }
}
