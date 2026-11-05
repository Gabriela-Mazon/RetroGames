using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = """
            SELECT
                co_jogo,
                co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var vinculos = new List<JogoGenero>();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero")
            });
        }

        return vinculos;
    }

    // Implementaremos os demais métodos nos próximos passos.

    public async Task<List<JogoGenero>> ListarPorJogosAsync(List<int> idsJogos)
{
    if (idsJogos.Count == 0)
    {
        return new List<JogoGenero>();
    }

    // Criar um parâmetro para cada ID informado.
    var marcadores = new List<string>();

    for (int i = 0; i < idsJogos.Count; i++)
    {
        marcadores.Add($"@idJogo{i}");
    }

    var sql = $"""
        SELECT
            co_jogo,
            co_genero
        FROM tb_jogo_genero
        WHERE co_jogo IN ({string.Join(", ", marcadores)})
        ORDER BY co_jogo, co_genero;
        """;

    await using var conexao = await AbrirConexaoAsync();
    await using var comando = CriarComando(conexao, sql);

    for (int i = 0; i < idsJogos.Count; i++)
    {
        AdicionarParametro(comando, $"@idJogo{i}", idsJogos[i]);
    }

    await using var leitor = await comando.ExecuteReaderAsync();

    var vinculos = new List<JogoGenero>();

    while (await leitor.ReadAsync())
    {
        vinculos.Add(new JogoGenero
        {
            IdJogo = leitor.GetInt32("co_jogo"),
            IdGenero = leitor.GetInt32("co_genero")
        });
    }

    return vinculos;
}

    public async Task InserirAsync(int idJogo, List<int> idsGeneros)
{
    if (idsGeneros.Count == 0)
    {
        return;
    }

    // Criar um marcador para cada gênero.
    var marcadores = new List<string>();

    for (int i = 0; i < idsGeneros.Count; i++)
    {
        marcadores.Add($"(@idJogo, @idGenero{i})");
    }

    var sql = $"""
        INSERT INTO tb_jogo_genero (co_jogo, co_genero)
        VALUES {string.Join(", ", marcadores)};
        """;

    await using var conexao = await AbrirConexaoAsync();
    await using var comando = CriarComando(conexao, sql);

    AdicionarParametro(comando, "@idJogo", idJogo);

    for (int i = 0; i < idsGeneros.Count; i++)
    {
        AdicionarParametro(comando, $"@idGenero{i}", idsGeneros[i]);
    }

    await ExecutarGravacaoAsync(comando);
}
    public async Task RemoverPorJogoAsync(int idJogo)
{
    const string sql = """
        DELETE FROM tb_jogo_genero
        WHERE co_jogo = @idJogo;
        """;

    await using var conexao = await AbrirConexaoAsync();
    await using var comando = CriarComando(conexao, sql);

    AdicionarParametro(comando, "@idJogo", idJogo);

    await comando.ExecuteNonQueryAsync();
}
}