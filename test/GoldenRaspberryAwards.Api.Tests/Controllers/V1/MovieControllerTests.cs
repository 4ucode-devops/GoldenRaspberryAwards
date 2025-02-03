using GoldenRaspberryAwards.Api.Controllers.V1;
using GoldenRaspberryAwards.Core.Interfaces;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Services;
using GoldenRaspberryAwards.Core.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace GoldenRaspberryAwards.Api.Tests.Controllers.V1;

public class MovieControllerTests
{
    private readonly INotifier _notifier;
    private readonly IAspNetUser _user;
    private readonly IMovieService _movieService;
    private readonly MovieController _controller;
    private readonly List<Notification> _notifications = new();

    public MovieControllerTests()
    {
        _notifier = Substitute.For<INotifier>();
        _user = Substitute.For<IAspNetUser>();
        _movieService = Substitute.For<IMovieService>();

        _notifier.When(n => n.Handle(Arg.Any<Notification>()))
                .Do(x => _notifications.Add(x.Arg<Notification>()));

        _notifier.GetNotifications().Returns(_notifications);

        _controller = new MovieController(_notifier, _user, _movieService);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Method = "GET" } }
        };
    }

    [Fact]
    public async Task GetProducersPrizeIntervals_ShouldReturnOk_WhenDataExists()
    {
        // Arrange
        var expectedResult = new { min = new List<object>(), max = new List<object>() };
        _movieService.GetProducersPrizeIntervals().Returns(Task.FromResult<object>(expectedResult));

        // Act
        var result = await _controller.GetProducersPrizeIntervals();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var responseType = okResult.Value.GetType();
        var successProperty = responseType.GetProperty("success");
        var dataProperty = responseType.GetProperty("data");

        Assert.NotNull(successProperty);
        Assert.NotNull(dataProperty);

        var successValue = (bool)successProperty.GetValue(okResult.Value);
        var dataValue = dataProperty.GetValue(okResult.Value);

        Assert.True(successValue);
        Assert.Equal(expectedResult, dataValue);
    }

    [Fact]
    public async Task GetProducersPrizeIntervals_ShouldReturnNotFound_WhenNoData()
    {
        // Arrange
        _movieService.GetProducersPrizeIntervals().Returns(Task.FromResult<object>(null));

        // Act
        var result = await _controller.GetProducersPrizeIntervals();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task GetProducersPrizeIntervals_ShouldReturnBadRequest_WhenExceptionOccurs()
    {
        // Arrange
        _movieService.GetProducersPrizeIntervals().Returns<Task<object>>(x => throw new Exception("Unexpected error"));

        // Act
        var result = await _controller.GetProducersPrizeIntervals();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }
}
