using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class ItemPedidoRepository(RepositoryConfig config) : RepositoryBase(config), IItemPedidoRepository
{
    public async Task<List<ItemPedido>> ListarPorPedidosAsync(IReadOnlyList<int> idsPedidos)
    {
        if (idsPedidos.Count == 0) return [];
        var marcadores = idsPedidos.Select((_, i) => $"@pedido{i}").ToArray();
        var sql = $"""
            SELECT co_seq_item_pedido, co_pedido, co_jogo, qt_item, vl_unitario
            FROM tb_item_pedido WHERE co_pedido IN ({string.Join(", ", marcadores)})
            ORDER BY co_pedido, co_seq_item_pedido;
            """;
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        for (int i = 0; i < idsPedidos.Count; i++)
            AdicionarParametro(comando, marcadores[i], idsPedidos[i]);

        await using var leitor = await comando.ExecuteReaderAsync();
        var itens = new List<ItemPedido>();
        while (await leitor.ReadAsync())
            itens.Add(new ItemPedido
            {
                Id = leitor.GetInt32(0), IdPedido = leitor.GetInt32(1), IdJogo = leitor.GetInt32(2),
                Quantidade = leitor.GetInt32(3), ValorUnitario = leitor.GetDecimal(4)
            });
        return itens;
    }

    public async Task InserirAsync(ItemPedido item)
    {
        const string sql = """
            INSERT INTO tb_item_pedido (qt_item, vl_unitario, co_pedido, co_jogo)
            VALUES (@quantidade, @preco, @pedido, @jogo);
            """;
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@quantidade", item.Quantidade);
        AdicionarParametro(comando, "@preco", item.ValorUnitario);
        AdicionarParametro(comando, "@pedido", item.IdPedido);
        AdicionarParametro(comando, "@jogo", item.IdJogo);
        await ExecutarGravacaoAsync(comando);
    }
}
