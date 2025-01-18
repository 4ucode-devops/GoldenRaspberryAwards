using FluentValidation;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwards.SharedServices.Services;

public class BaseService
{
    protected readonly INotifier _notifier;

    public BaseService(INotifier notifier)
    {
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    public object NotificationType { get; private set; }

    protected async Task<List<string>> ExecuteValidationAsync<TV, TE>(TV validation, TE entity)
        where TV : AbstractValidator<TE>
        where TE : EntityBase
    {
        var validationResult = await validation.ValidateAsync(entity);
        var errors = new List<string>();

        if (validationResult.IsValid) return errors;

        foreach (var error in validationResult.Errors)
        {
            var errorMessage = $"Property: {error.PropertyName} - Error: {error.ErrorMessage}";
            _notifier.Handle(errorMessage);
            errors.Add(errorMessage);
        }

        return errors;
    }

    protected void HandleException(Exception exception)
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));

        string friendlyMessage = GenerateFriendlyMessage(exception);
        _notifier.Handle(friendlyMessage);
    }

    private string GenerateFriendlyMessage(Exception exception) => exception switch
    {
        ArgumentNullException argNullException =>
            $"A required argument was not provided: {argNullException.ParamName}. Please check and try again.",

        ArgumentException argException =>
            $"There was an issue with the provided argument: {argException.Message}. Please correct it and try again.",

        InvalidOperationException invalidOpException =>
            $"The operation could not be completed: {invalidOpException.Message}. Please ensure all preconditions are met and try again.",

        DbUpdateException dbUpdateException =>
            $"Database update error: {dbUpdateException.InnerException?.Message ?? dbUpdateException.Message}. Please contact support if the issue persists.",

        _ => $"An unexpected error occurred: {exception.Message}. Please try again later or contact support."
    };
}