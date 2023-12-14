using FluentValidation;
using WebServer.Models;

namespace WebServer.Validation;

public class PostValidator: AbstractValidator<Post>
{
    public PostValidator()
    {
        RuleFor(post => post.Title)
            .NotEmpty().WithMessage("Это поле должно быть заполнено");
        RuleFor(post => post.Content)
            .NotEmpty().WithMessage("Это поле должно быть заполнено");
    }  
}