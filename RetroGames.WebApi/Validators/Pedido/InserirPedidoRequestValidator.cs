using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Pedido;

public class InserirPedidoRequestValidator : AbstractValidator<InserirPedidoRequestApiModel>
{
    public InserirPedidoRequestValidator()
    {
        RuleFor(x => x.Itens).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Adicione ao menos um jogo ao carrinho.")
            .Must(itens => itens.All(i => i is not null))
            .WithMessage("Informe o jogo e a quantidade de cada item.")
            .Must(itens => itens.DistinctBy(i => i.IdJogo).Count() == itens.Count)
            .WithMessage("Reúna as unidades de um mesmo jogo em um único item.");
        RuleForEach(x => x.Itens).ChildRules(item =>
            item.RuleFor(i => i.Quantidade).InclusiveBetween(1, 10)
                .WithMessage("Escolha de 1 a 10 unidades por jogo."));
    }
}
