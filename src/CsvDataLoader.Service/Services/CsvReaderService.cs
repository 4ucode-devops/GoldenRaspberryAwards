using CsvHelper;
using CsvHelper.Configuration;
using GoldenRaspberryAwards.Core.Interfaces;
using System.Globalization;

public class CsvProcessor<T, TCsv> : ICsvProcessor<T>
    where T : class
    where TCsv : class, new()
{
    private readonly IEntityValidator<T> _validator;
    private readonly INotifier _notifier;
    private readonly Func<TCsv, T> _mapper;

    public CsvProcessor(IEntityValidator<T> validator, INotifier notifier, Func<TCsv, T> mapper)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<T>> ProcessCsvAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            _notifier.Handle("File path is required.");
            return new List<T>();
        }

        if (!File.Exists(filePath))
        {
            _notifier.Handle($"File not found at path: {filePath}");
            return new List<T>();
        }

        var entities = new List<T>();
        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            });

            var records = csv.GetRecordsAsync<TCsv>();
            await foreach (var record in records)
            {
                var entity = _mapper(record);
                var validationErrors = _validator.Validate(entity);

                if (validationErrors.Any())
                {
                    foreach (var error in validationErrors)
                        _notifier.Handle(error);

                    continue;
                }

                entities.Add(entity);
            }
        }
        catch (Exception ex)
        {
            _notifier.Handle($"An error occurred while reading the CSV file: {ex.Message}");
        }

        return entities;
    }
}
