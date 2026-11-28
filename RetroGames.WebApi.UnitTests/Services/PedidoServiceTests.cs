using System.Transactions;
using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class PedidoServiceTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IPedidoRepository> _pedidos = new();
    private readonly Mock<IItemPedidoRepository> _itens = new();
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        var repos = new Mock<IRepositorySet>();
        repos.SetupGet(x => x.Jogo).Returns(_jogos.Object);
        repos.SetupGet(x => x.Pedido).Returns(_pedidos.Object);
        repos.SetupGet(x => x.ItemPedido).Returns(_itens.Object);
        _service = new PedidoService(repos.Object);
    }

    [Theory]
    [InlineData()]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(6, 6)]
    public async Task Finalizar_RecusaCarrinhoInvalidoAntesDeGravar(params int[] quantidades)
    {
        var carrinho = quantidades.Select(q => new ItemCompraServiceModel(1, q)).ToList();
        await Assert.ThrowsAsync<ArgumentException>(() => _service.FinalizarAsync(7, carrinho));
        _jogos.VerifyNoOtherCalls();
        _pedidos.VerifyNoOtherCalls();
        _itens.VerifyNoOtherCalls();
    }

    private void Oferecer(int id, decimal preco, int estoque, bool ativo = true) =>
        _jogos.Setup(x => x.BuscarParaCompraAsync(id)).ReturnsAsync(
            new Jogo { Id = id, Preco = preco, Estoque = estoque, Ativo = ativo });

    [Fact]
    public async Task Finalizar_CalculaPelosPrecosDoBanco_GravaItensEBaixaEstoque()
    {
        Oferecer(1, 19.95m, 2);
        Oferecer(2, 10.05m, 6);
        _pedidos.Setup(x => x.InserirAsync(It.IsAny<Pedido>())).ReturnsAsync(12);
        var inicio = DateTime.UtcNow;

        var pedido = await _service.FinalizarAsync(7, [new(2, 3), new(1, 2)]);

        Assert.Equal(12, pedido.Id);
        Assert.Equal(7, pedido.IdUsuario);
        Assert.Equal(70.05m, pedido.Total);
        Assert.Equal(SituacaoPedidoIds.Pendente, pedido.IdSituacaoPedido);
        Assert.Equal(DateTimeKind.Utc, pedido.Data.Kind);
        Assert.InRange(pedido.Data, inicio, DateTime.UtcNow);
        _itens.Verify(x => x.InserirAsync(It.Is<ItemPedido>(i =>
            i.IdPedido == 12 && i.IdJogo == 1 && i.Quantidade == 2 && i.ValorUnitario == 19.95m)), Times.Once);
        _itens.Verify(x => x.InserirAsync(It.Is<ItemPedido>(i =>
            i.IdPedido == 12 && i.IdJogo == 2 && i.Quantidade == 3 && i.ValorUnitario == 10.05m)), Times.Once);
        _jogos.Verify(x => x.BaixarEstoqueAsync(1, 2), Times.Once);
        _jogos.Verify(x => x.BaixarEstoqueAsync(2, 3), Times.Once);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Finalizar_JogoAusenteOuInativo_Erro110(bool existe)
    {
        if (existe) Oferecer(1, 12m, 4, ativo: false);
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _service.FinalizarAsync(7, [new(1, 1)]));
        Assert.Equal(BusinessErrors.JOGO_INDISPONIVEL, erro.Erro);
        _jogos.Verify(x => x.BaixarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _pedidos.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Finalizar_SegundoJogoSemEstoque_AbortaTransacaoDaPrimeiraBaixa()
    {
        Oferecer(1, 10m, 2);
        Oferecer(2, 12m, 0);
        TransactionStatus? resultado = null;
        _jogos.Setup(x => x.BaixarEstoqueAsync(1, 1)).Callback(() =>
            Transaction.Current!.TransactionCompleted += (_, e) =>
                resultado = e.Transaction!.TransactionInformation.Status).Returns(Task.CompletedTask);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _service.FinalizarAsync(7, [new(2, 1), new(1, 1)]));

        Assert.Equal(BusinessErrors.ESTOQUE_INSUFICIENTE, erro.Erro);
        Assert.Equal(TransactionStatus.Aborted, resultado);
        _pedidos.VerifyNoOtherCalls();
        _itens.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Finalizar_FalhaAoGravarItem_AbortaInclusiveOCabecalho()
    {
        Oferecer(1, 10m, 2);
        TransactionStatus? resultado = null;
        _pedidos.Setup(x => x.InserirAsync(It.IsAny<Pedido>())).Callback(() =>
            Transaction.Current!.TransactionCompleted += (_, e) =>
                resultado = e.Transaction!.TransactionInformation.Status).ReturnsAsync(12);
        _itens.Setup(x => x.InserirAsync(It.IsAny<ItemPedido>())).ThrowsAsync(new IOException());

        await Assert.ThrowsAsync<IOException>(() => _service.FinalizarAsync(7, [new(1, 1)]));
        Assert.Equal(TransactionStatus.Aborted, resultado);
    }
}
