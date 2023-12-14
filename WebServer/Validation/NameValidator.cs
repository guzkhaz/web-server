using FluentValidation;
using WebServer.Models;

namespace WebServer.Validation;

public class NameValidator: AbstractValidator<User>
{
    public NameValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Это поле должно быть заполнено")
            .MinimumLength(2).WithMessage("Длина имени должна быть не менее 2 символов")
            .MaximumLength(20).WithMessage("Длина имени должна быть не более 20 символов")
            .Matches("[А-ЯA-Z][а-яА-ЯёЁa-zA-Z]");
    }
}