using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class SituacaoPedidoRepository(RepositoryConfig config) : RepositoryBase(config), ISituacaoPedidoRepository
{
    public async Task<List<SituacaoPedido>> ListarAsync()
    {
        const string sql = "SELECT co_situacao_pedido, ds_situacao_pedido FROM tb_situacao_pedido ORDER BY co_situacao_pedido";
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();
        var situacoes = new List<SituacaoPedido>();
        while (await leitor.ReadAsync())
            situacoes.Add(new SituacaoPedido { Id = leitor.GetInt32(0), Descricao = leitor.GetString(1) });
        return situacoes;
    }
}
