using FluentValidation;
using WebServer.Models;

namespace WebServer.Validation;

public class CommentValidator:AbstractValidator<Comment>
{
    public CommentValidator()
    {
        RuleFor(comment => comment.Content)
            .NotEmpty().WithMessage("Это поле должно быть заполнено");
    }
}