using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Autenticacao;

public class CadastrarUsuarioRequestValidator : AbstractValidator<CadastrarUsuarioRequestApiModel>
{
    public CadastrarUsuarioRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(150).WithMessage("Use até 150 caracteres no nome.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Informe o e-mail.")
            .EmailAddress().WithMessage("Informe um e-mail válido.")
            .MaximumLength(255).WithMessage("Use até 255 caracteres no e-mail.");
        RuleFor(x => x.Senha).NotEmpty().WithMessage("Informe uma senha.");
    }
}
