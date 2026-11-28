using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class MeusPedidosServiceTests
{
    [Fact]
    public async Task SemPedidos_ConsultaSomenteOClienteSolicitado()
    {
        var repos = new Mock<IRepositorySet>(MockBehavior.Strict);
        var pedidos = new Mock<IPedidoRepository>(MockBehavior.Strict);
        pedidos.Setup(p => p.ListarPorUsuarioAsync(9)).ReturnsAsync([]);
        repos.SetupGet(r => r.Pedido).Returns(pedidos.Object);

        Assert.Empty(await new PedidoService(repos.Object).ListarMeusAsync(9));
        pedidos.Verify(p => p.ListarPorUsuarioAsync(9), Times.Once);
    }

    [Fact]
    public async Task Historico_PreservaOrdemESituacao_UsaPrecoPagoMesmoComJogoDesativado()
    {
        var repos = new Mock<IRepositorySet> { DefaultValue = DefaultValue.Mock };
        var hoje = DateTime.UtcNow;
        repos.Setup(r => r.Pedido.ListarPorUsuarioAsync(7)).ReturnsAsync([
            new Pedido { Id = 2, IdUsuario = 7, Data = hoje, Total = 18, IdSituacaoPedido = 1 },
            new Pedido { Id = 1, IdUsuario = 7, Data = hoje.AddDays(-1), Total = 7, IdSituacaoPedido = 2 }
        ]);
        repos.Setup(r => r.ItemPedido.ListarPorPedidosAsync(It.Is<IReadOnlyList<int>>(
            ids => ids.SequenceEqual(new[] { 2, 1 })))).ReturnsAsync([
            new ItemPedido { IdPedido = 1, IdJogo = 3, Quantidade = 1, ValorUnitario = 7 },
            new ItemPedido { IdPedido = 2, IdJogo = 3, Quantidade = 2, ValorUnitario = 9 }
        ]);
        repos.Setup(r => r.Jogo.ListarAsync(null, null, It.Is<List<int>>(ids => ids.SequenceEqual(new[] { 3 })), 1, 1, true))
            .ReturnsAsync((new List<Jogo> { new() { Id = 3, Nome = "Jogo de teste", Preco = 99, Ativo = false } }, 1));
        repos.Setup(r => r.SituacaoPedido.ListarAsync()).ReturnsAsync([
            new SituacaoPedido { Id = 1, Descricao = "Pendente" }, new SituacaoPedido { Id = 2, Descricao = "Pago" }
        ]);

        var lista = await new PedidoService(repos.Object).ListarMeusAsync(7);

        Assert.Equal(new[] { 2, 1 }, lista.Select(p => p.Id));
        Assert.Equal(new[] { "Pendente", "Pago" }, lista.Select(p => p.Situacao));
        Assert.Equal(18, lista[0].Total);
        Assert.Equal(new ItemPedidoResumoServiceModel(3, "Jogo de teste", 2, 9), Assert.Single(lista[0].Itens));
        Assert.Equal(7, Assert.Single(lista[1].Itens).ValorUnitario);
    }
}
