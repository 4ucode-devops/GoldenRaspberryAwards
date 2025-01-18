namespace GoldenRaspberryAwards.Core.Interfaces.Services;

public interface ICsvProcessorService<T> where T : class
{
    Task<List<T>> ProcessCsvAsync(string filePath);
}
