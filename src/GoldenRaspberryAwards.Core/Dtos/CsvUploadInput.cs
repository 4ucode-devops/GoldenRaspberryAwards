using Microsoft.AspNetCore.Http;

namespace GoldenRaspberryAwards.Core.Dtos;

public class CsvUploadInput
{
    public IFormFile File { get; set; }
}
