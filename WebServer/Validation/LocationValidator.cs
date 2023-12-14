using FluentValidation;
using WebServer.Models;

namespace WebServer.Validation;

public class LocationValidator: AbstractValidator<User>
{
    public LocationValidator()
    {
        RuleFor(user => user.Location)
            .MinimumLength(2).WithMessage("Длина названия города должна быть не менее 2 символов")
            .MaximumLength(20).WithMessage("Длина названия города должна быть не более 20 символов")
            .Matches("[А-ЯA-Z][а-яА-ЯёЁa-zA-Z]");
    }
}