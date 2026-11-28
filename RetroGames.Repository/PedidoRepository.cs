using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PedidoRepository(RepositoryConfig config) : RepositoryBase(config), IPedidoRepository
{
    public async Task<int> InserirAsync(Pedido pedido)
    {
        const string sql = """
            INSERT INTO tb_pedido (dt_pedido, vl_total, co_usuario, co_situacao_pedido)
            VALUES (@data, @total, @usuario, @situacao);
            """;
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@data", pedido.Data);
        AdicionarParametro(comando, "@total", pedido.Total);
        AdicionarParametro(comando, "@usuario", pedido.IdUsuario);
        AdicionarParametro(comando, "@situacao", pedido.IdSituacaoPedido);
        await ExecutarGravacaoAsync(comando);
        return checked((int)comando.LastInsertedId);
    }
}
