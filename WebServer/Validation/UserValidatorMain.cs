using FluentValidation;
using WebServer.Models;

namespace WebServer.Validation;


public class UserValidatorMain: AbstractValidator<User>
{
    public UserValidatorMain()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Это поле должно быть заполнено")
            .EmailAddress().WithMessage("Почта должна быть представлена в формате вида login@example.com");
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Это поле должно быть заполнено")
            .MinimumLength(2).WithMessage("Длина имени должна быть не менее 2 символов")
            .MaximumLength(20).WithMessage("Длина имени должна быть не более 20 символов")
            .Matches("[А-ЯA-Z][а-яА-ЯёЁa-zA-Z]");
        RuleFor(user => user.Surname)
            .NotEmpty().WithMessage("Это поле должно быть заполнено")
            .MinimumLength(2).WithMessage("Длина фамилии должна быть не менее 2 символов")
            .MaximumLength(20).WithMessage("Длина фамилии должна быть не более 20 символов")
            .Matches("[А-ЯA-Z][а-яА-ЯёЁa-zA-Z]");
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Это поле должно быть заполнено")
            .Password().WithMessage("Пароль должен содержать от 8 до 16 символов и состоять, по крайней мере, " +
                                    "из одного числа, одной прописной и одной строчной букв и специального символа");
    }   
}