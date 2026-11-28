using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PedidoRepository(RepositoryConfig config) : RepositoryBase(config), IPedidoRepository
{
    public async Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario)
    {
        const string sql = """
            SELECT co_seq_pedido, dt_pedido, vl_total, co_situacao_pedido
            FROM tb_pedido WHERE co_usuario = @usuario
            ORDER BY dt_pedido DESC, co_seq_pedido DESC;
            """;
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@usuario", idUsuario);
        await using var leitor = await comando.ExecuteReaderAsync();
        var pedidos = new List<Pedido>();
        while (await leitor.ReadAsync())
            pedidos.Add(new Pedido
            {
                Id = leitor.GetInt32(0), Data = DateTime.SpecifyKind(leitor.GetDateTime(1), DateTimeKind.Utc),
                Total = leitor.GetDecimal(2), IdSituacaoPedido = leitor.GetInt32(3), IdUsuario = idUsuario
            });
        return pedidos;
    }

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
