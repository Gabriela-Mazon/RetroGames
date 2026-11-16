using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Jogo;

public class InserirJogoRequestValidator
    : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(modelo => modelo.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome.")
            .MaximumLength(150)
            .WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(modelo => modelo.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição deve ter no máximo 1000 caracteres.");

        RuleFor(modelo => modelo.AnoLancamento)
            .InclusiveBetween(1970, 2100)
            .WithMessage("Ano de lançamento inválido.");

        RuleFor(modelo => modelo.Preco)
            .GreaterThan(0m)
            .WithMessage("O preço deve ser maior que zero.")
            .PrecisionScale(10, 2, true)
            .WithMessage(
                "O preço deve ter no máximo 8 dígitos inteiros e 2 casas decimais.");

        RuleFor(modelo => modelo.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo.");

        RuleFor(modelo => modelo.IdPlataforma)
            .GreaterThan(0)
            .WithMessage("Informe a plataforma.");

        RuleFor(modelo => modelo.IdCondicaoJogo)
            .GreaterThan(0)
            .WithMessage("Informe a condição do jogo.");

        RuleFor(modelo => modelo.IdsGeneros)
            .NotEmpty()
            .WithMessage("Informe pelo menos um gênero.");
    }
}