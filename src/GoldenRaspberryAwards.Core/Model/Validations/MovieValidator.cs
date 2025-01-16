using FluentValidation;

namespace GoldenRaspberryAwards.Core.Model.Validations;

public class MovieValidator
{
    private readonly AbstractValidator<Movie> _fluentValidator;
    public MovieValidator()
    {
        _fluentValidator = new InlineValidator<Movie>()
        {
            v => v.RuleFor(m => m.Title).NotEmpty().WithMessage("The title is required."),
            v => v.RuleFor(m => m.Year).InclusiveBetween(1900, DateTime.Now.Year).WithMessage("Year must be between 1900 and the current year."),
            v => v.RuleFor(m => m.MovieProducers).NotEmpty().WithMessage("At least one producer is required.")
        };
    }

    public List<string> Validate(Movie entity)
    {
        var results = _fluentValidator.Validate(entity);
       return results.IsValid?new List<string>(): results.Errors.Select(e => e.ErrorMessage).ToList();
    }
}
