
using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Autenticacao;

public class LoginRequestValidator
    : AbstractValidator<LoginRequestApiModel>
{
    public LoginRequestValidator()
    {
        RuleFor(modelo => modelo.Email)
            .NotEmpty()
            .WithMessage("Informe o e-mail.")
            .EmailAddress()
            .WithMessage("Informe um e-mail válido.");

        RuleFor(modelo => modelo.Senha)
            .NotEmpty()
            .WithMessage("Informe a senha.");
    }
}
