using FluentValidation;

namespace WebServer.Validation;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        var options = ruleBuilder
            .MinimumLength(8).WithMessage("Длина пароля должна быть не менее 8 символов")
            .MaximumLength(16). WithMessage("Длина пароля должна быть не менее 16 символов")
            .Matches("[A-Z]").WithMessage("Пароль должен содержать одну заглавную букву")
            .Matches("[a-z]").WithMessage("Пароль должен содержать одну строчную букву")
            .Matches("[0-9]").WithMessage("Пароль должен содержать одну цифру")
            .Matches("[^a-zA-z0-9]").WithMessage("Пароль должен содержать один специальный символ");
        return options;
    }
}