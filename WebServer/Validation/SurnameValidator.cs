using FluentValidation;
using WebServer.Models;

namespace WebServer.Validation;

public class SurnameValidator: AbstractValidator<User>
{
    public SurnameValidator()
    {
        RuleFor(user => user.Surname)
            .NotEmpty().WithMessage("Это поле должно быть заполнено")
            .MinimumLength(2).WithMessage("Длина фамилии должна быть не менее 2 символов")
            .MaximumLength(20).WithMessage("Длина фамилии должна быть не более 20 символов")
            .Matches("[А-ЯA-Z][а-яА-ЯёЁa-zA-Z]");
    }
}