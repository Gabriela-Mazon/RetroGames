using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Validators.Pedido;

namespace RetroGames.WebApi.UnitTests.Validators;

public class InserirPedidoValidatorTests
{
    private readonly InserirPedidoRequestValidator _validator = new();

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(10, true)]
    [InlineData(11, false)]
    public void QuantidadePorJogo_RespeitaOsLimites(int quantidade, bool aceito)
    {
        var entrada = new InserirPedidoRequestApiModel { Itens = [new() { IdJogo = 4, Quantidade = quantidade }] };
        Assert.Equal(aceito, _validator.Validate(entrada).IsValid);
    }

    [Fact]
    public void CarrinhoVazioOuComJogoRepetido_FalhaNaValidacao()
    {
        var entrada = new InserirPedidoRequestApiModel();
        Assert.False(_validator.Validate(entrada).IsValid);
        var item = new ItemCompraRequestApiModel { IdJogo = 1, Quantidade = 6 };
        entrada.Itens = [item, item];
        Assert.False(_validator.Validate(entrada).IsValid);
    }
}
