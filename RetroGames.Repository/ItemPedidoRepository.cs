using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class ItemPedidoRepository(RepositoryConfig config) : RepositoryBase(config), IItemPedidoRepository
{
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
