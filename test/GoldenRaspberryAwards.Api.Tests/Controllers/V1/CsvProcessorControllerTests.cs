using AutoMapper;
using FluentAssertions;
using GoldenRaspberryAwards.Api.Controllers.V1;
using GoldenRaspberryAwards.Core.Dtos;
using GoldenRaspberryAwards.Core.Enum;
using GoldenRaspberryAwards.Core.Interfaces;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Services;
using GoldenRaspberryAwards.Core.Model;
using GoldenRaspberryAwards.Core.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace GoldenRaspberryAwards.Api.Tests.Controllers.V1;

public class CsvProcessorControllerTests
{
    private readonly ICsvProcessorService<Movie> _csvProcessorService;
    private readonly IMapper _mapper;
    private readonly INotifier _notifier;
    private readonly IAspNetUser _user;
    private readonly CsvProcessorController _controller;

    public CsvProcessorControllerTests()
    {
        _csvProcessorService = Substitute.For<ICsvProcessorService<Movie>>();
        _mapper = Substitute.For<IMapper>();
        _notifier = Substitute.For<INotifier>();
        _user = Substitute.For<IAspNetUser>();

        _controller = new CsvProcessorController(
            _notifier,
            _user,
            _csvProcessorService,
            _mapper);
    }

    [Fact]
    public async Task ProcessCsv_ArquivoNulo_DeveRetornarBadRequest()
    {
        // Arrange
        var input = new CsvUploadInput { File = null };

        // Act
        var result = await _controller.ProcessCsv(input);

        // Assert
        _notifier.Received(1).Handle(
            Arg.Is<Notification>(nt =>
                nt.Message == "Error: Arquivo CSV é obrigatório." &&
                nt.Type == NotificationType.Error));

        AssertResultBadRequest(result, "Error: Arquivo CSV é obrigatório.");
    }

    [Fact]
    public async Task ProcessCsv_ArquivoVazio_DeveRetornarBadRequest()
    {
        // Arrange
        var fileMock = CriarArquivoMock(0);
        var input = new CsvUploadInput { File = fileMock };

        // Act
        var result = await _controller.ProcessCsv(input);

        // Assert
        _notifier.Received(1).Handle(
            Arg.Is<Notification>(nt =>
                nt.Message == "Error: Arquivo CSV é obrigatório." &&
                nt.Type == NotificationType.Error));

        AssertResultBadRequest(result, "Error: Arquivo CSV é obrigatório.");
    }

    [Fact]
    public async Task ProcessCsv_ProcessamentoSucesso_DeveRetornarCreatedComDados()
    {
        // Arrange
        var fileMock = CriarArquivoMock(1024);
        var input = new CsvUploadInput { File = fileMock };

        var movies = new List<Movie> { new Movie() };
        var moviesDto = new List<MovieDTO> { new MovieDTO() };

        _csvProcessorService.ProcessCsvAsync(Arg.Any<string>()).Returns(movies);
        _mapper.Map<List<MovieDTO>>(movies).Returns(moviesDto);

        // Act
        var result = await _controller.ProcessCsv(input);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);

        dynamic response = objectResult.Value;
        Assert.True(response.success);
        Assert.Equal(moviesDto, response.data);
    }

    [Fact]
    public async Task ProcessCsv_NenhumRegistroImportado_DeveRetornarMensagemEspecifica()
    {
        // Arrange
        var fileMock = CriarArquivoMock(1024);
        var input = new CsvUploadInput { File = fileMock };

        _csvProcessorService.ProcessCsvAsync(Arg.Any<string>()).Returns(new List<Movie>());

        // Act
        var result = await _controller.ProcessCsv(input);

        // Assert
        AssertResultBadRequest(result, "Nenhum registro importado (ou todos já existiam).");
    }

    [Fact]
    public async Task ProcessCsv_ExcecaoDuranteProcessamento_DeveRetornarInternalServerError()
    {
        // Arrange
        var fileMock = CriarArquivoMock(1024);
        var input = new CsvUploadInput { File = fileMock };
        var exceptionMessage = "Erro simulado";

        _csvProcessorService.ProcessCsvAsync(Arg.Any<string>()).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.ProcessCsv(input);

        // Assert
        _notifier.Received(1).Handle(
            Arg.Is<Notification>(nt =>
                nt.Message == exceptionMessage &&
                nt.Type == NotificationType.Error));

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

        dynamic response = objectResult.Value;
        Assert.False(response.success);
        Assert.Contains(exceptionMessage, (IEnumerable<string>)response.errors);
    }

    private IFormFile CriarArquivoMock(long length)
    {
        var fileMock = Substitute.For<IFormFile>();
        fileMock.FileName.Returns("filmes.csv");
        fileMock.Length.Returns(length);
        return fileMock;
    }

    private void AssertResultBadRequest(IActionResult result, string expectedMessage)
    {
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        dynamic response = badRequestResult.Value;
        Assert.False(response.success);
        Assert.Equal(expectedMessage, response.errors);
    }
}
