using FluentValidation;
using WebServer.Models;

namespace WebServer.Validation;

public class LoginValidator: AbstractValidator<LoginModel>
{
    public LoginValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Это поле должно быть заполнено")
            .EmailAddress().WithMessage("Почта должна быть представлена в формате вида login@example.com");
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Это поле должно быть заполнено")
            .Password().WithMessage("Пароль должен содержать от 8 до 16 символов и состоять, по крайней мере, " +
                                    "из одного числа, одной прописной и одной строчной букв и специального символа");
    }  
}