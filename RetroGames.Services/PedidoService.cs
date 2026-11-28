using System.Transactions;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PedidoService(IRepositorySet repositorio) : IPedidoService
{
    public async Task<List<PedidoResumoServiceModel>> ListarMeusAsync(int idUsuario)
    {
        var pedidos = await repositorio.Pedido.ListarPorUsuarioAsync(idUsuario);
        if (pedidos.Count == 0) return [];

        var idsPedidos = pedidos.Select(p => p.Id).ToList();
        var itens = await repositorio.ItemPedido.ListarPorPedidosAsync(idsPedidos);
        var idsJogos = itens.Select(i => i.IdJogo).Distinct().ToList();
        // Um jogo desativado continua fazendo parte do histórico de quem o comprou.
        var (jogos, _) = await repositorio.Jogo.ListarAsync(ids: idsJogos, tamanhoPagina: idsJogos.Count, incluirInativos: true);
        var nomes = jogos.ToDictionary(j => j.Id, j => j.Nome);
        var situacoes = (await repositorio.SituacaoPedido.ListarAsync()).ToDictionary(s => s.Id, s => s.Descricao);
        var itensPorPedido = itens.ToLookup(i => i.IdPedido);
        return pedidos.Select(p => new PedidoResumoServiceModel(
            p.Id, p.Data, situacoes[p.IdSituacaoPedido], p.Total, itensPorPedido[p.Id].Select(i =>
                new ItemPedidoResumoServiceModel(i.IdJogo, nomes[i.IdJogo], i.Quantidade, i.ValorUnitario)).ToList()
        )).ToList();
    }

    public async Task<Pedido> FinalizarAsync(int idUsuario, IReadOnlyList<ItemCompraServiceModel> carrinho)
    {
        // Pré-condições também protegem chamadas diretas ao Service.
        if (carrinho.Count == 0 || carrinho.Any(i => i.Quantidade is < 1 or > 10) ||
            carrinho.DistinctBy(i => i.IdJogo).Count() != carrinho.Count)
            throw new ArgumentException("O carrinho deve ter jogos distintos e 1 a 10 unidades por jogo.", nameof(carrinho));

        var opcoes = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        using var transacao = new TransactionScope(TransactionScopeOption.Required,
            opcoes, TransactionScopeAsyncFlowOption.Enabled);

        var itens = new List<ItemPedido>();
        // Travar sempre na mesma ordem evita disputa circular entre dois carrinhos.
        foreach (var escolha in carrinho.OrderBy(item => item.IdJogo))
        {
            var jogo = await repositorio.Jogo.BuscarParaCompraAsync(escolha.IdJogo);
            if (jogo is null || !jogo.Ativo)
                throw new BusinessRuleException(BusinessErrors.JOGO_INDISPONIVEL);
            if (jogo.Estoque < escolha.Quantidade)
                throw new BusinessRuleException(BusinessErrors.ESTOQUE_INSUFICIENTE);

            itens.Add(new ItemPedido
            {
                IdJogo = jogo.Id,
                Quantidade = escolha.Quantidade,
                ValorUnitario = jogo.Preco
            });
            await repositorio.Jogo.BaixarEstoqueAsync(jogo.Id, escolha.Quantidade);
        }

        var pedido = new Pedido
        {
            Data = DateTime.UtcNow,
            Total = itens.Sum(item => item.Quantidade * item.ValorUnitario),
            IdUsuario = idUsuario,
            IdSituacaoPedido = SituacaoPedidoIds.Pendente
        };
        pedido.Id = await repositorio.Pedido.InserirAsync(pedido);
        foreach (var item in itens)
        {
            item.IdPedido = pedido.Id;
            await repositorio.ItemPedido.InserirAsync(item);
        }
        transacao.Complete();
        return pedido;
    }
}
