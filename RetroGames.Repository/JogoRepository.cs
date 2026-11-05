using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        List<int>? ids = null,
        int pagina = 1,
        int tamanhoPagina = 10,
        bool incluirInativos = false)
    {
        var filtros = new List<string>();
        var parametros = new List<(string Nome, object Valor)>();

        // Por padrão, não mostrar jogos inativos.
        if (!incluirInativos)
        {
            filtros.Add("st_ativo = TRUE");
        }

        // Filtro por parte do nome.
        if (!string.IsNullOrWhiteSpace(busca))
        {
            filtros.Add("no_jogo LIKE @busca");
            parametros.Add(("@busca", $"%{busca.Trim()}%"));
        }

        // Filtro por plataforma.
        if (idPlataforma.HasValue)
        {
            filtros.Add("co_plataforma = @idPlataforma");
            parametros.Add(("@idPlataforma", idPlataforma.Value));
        }

        // Filtro por lista de ids de jogos.
        if (ids is not null)
        {
            if (ids.Count == 0)
            {
                return (new List<Jogo>(), 0);
            }

            var marcadores = new List<string>();

            for (int i = 0; i < ids.Count; i++)
            {
                var marcador = $"@idJogo{i}";
                marcadores.Add(marcador);
                parametros.Add((marcador, ids[i]));
            }

            filtros.Add(
                $"co_seq_jogo IN ({string.Join(", ", marcadores)})");
        }

        // Os mesmos filtros serão usados nas duas consultas.
        var filtroSql = filtros.Count == 0
            ? ""
            : " WHERE " + string.Join(" AND ", filtros);

        var sqlTotal = $"SELECT COUNT(*) FROM tb_jogo{filtroSql};";

        var sqlListagem = $"""
            SELECT
                co_seq_jogo,
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            FROM tb_jogo{filtroSql}
            ORDER BY no_jogo, co_seq_jogo
            LIMIT @limite OFFSET @deslocamento;
            """;

        await using var conexao = await AbrirConexaoAsync();

        // Primeiro, contar o total de jogos encontrados.
        int total;

        await using (var comandoTotal = CriarComando(conexao, sqlTotal))
        {
            foreach (var parametro in parametros)
            {
                AdicionarParametro(
                    comandoTotal, parametro.Nome, parametro.Valor);
            }

            total = Convert.ToInt32(
                await comandoTotal.ExecuteScalarAsync());
        }

        // Depois, buscar apenas a página solicitada.
        await using var comando = CriarComando(conexao, sqlListagem);

        foreach (var parametro in parametros)
        {
            AdicionarParametro(
                comando, parametro.Nome, parametro.Valor);
        }

        AdicionarParametro(comando, "@limite", tamanhoPagina);
        AdicionarParametro(
            comando, "@deslocamento", (pagina - 1) * tamanhoPagina);

        await using var leitor = await comando.ExecuteReaderAsync();

        var jogos = new List<Jogo>();

        while (await leitor.ReadAsync())
        {
            jogos.Add(new Jogo
            {
                Id = leitor.GetInt32("co_seq_jogo"),
                Nome = leitor.GetString("no_jogo"),

                Descricao = leitor.IsDBNull(
                    leitor.GetOrdinal("ds_jogo"))
                    ? null
                    : leitor.GetString("ds_jogo"),

                AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
                Preco = leitor.GetDecimal("vl_preco"),
                Estoque = leitor.GetInt32("qt_estoque"),
                Ativo = leitor.GetBoolean("st_ativo"),
                DataCadastro = leitor.GetDateTime("dt_cadastro"),
                IdPlataforma = leitor.GetInt32("co_plataforma"),
                IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo")
            });
        }

        return (jogos, total);
    }

    // Implementaremos estes métodos nos próximos passos.

    public async Task<Jogo?> BuscarPorIdAsync(int id)
{
    const string sql = """
        SELECT
            co_seq_jogo,
            no_jogo,
            ds_jogo,
            nu_ano_lancamento,
            vl_preco,
            qt_estoque,
            st_ativo,
            dt_cadastro,
            co_plataforma,
            co_condicao_jogo
        FROM tb_jogo
        WHERE co_seq_jogo = @id;
        """;

    await using var conexao = await AbrirConexaoAsync();
    await using var comando = CriarComando(conexao, sql);

    AdicionarParametro(comando, "@id", id);

    await using var leitor = await comando.ExecuteReaderAsync();

    if (!await leitor.ReadAsync())
    {
        return null;
    }

    return new Jogo
    {
        Id = leitor.GetInt32("co_seq_jogo"),
        Nome = leitor.GetString("no_jogo"),

        Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo"))
            ? null
            : leitor.GetString("ds_jogo"),

        AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
        Preco = leitor.GetDecimal("vl_preco"),
        Estoque = leitor.GetInt32("qt_estoque"),
        Ativo = leitor.GetBoolean("st_ativo"),
        DataCadastro = leitor.GetDateTime("dt_cadastro"),
        IdPlataforma = leitor.GetInt32("co_plataforma"),
        IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo")
    };
}

    public async Task<int> InserirAsync(Jogo jogo)
{
    const string sql = """
        INSERT INTO tb_jogo (
            no_jogo,
            ds_jogo,
            nu_ano_lancamento,
            vl_preco,
            qt_estoque,
            st_ativo,
            dt_cadastro,
            co_plataforma,
            co_condicao_jogo
        )
        VALUES (
            @nome,
            @descricao,
            @anoLancamento,
            @preco,
            @estoque,
            @ativo,
            UTC_TIMESTAMP(),
            @idPlataforma,
            @idCondicaoJogo
        );
        """;

    await using var conexao = await AbrirConexaoAsync();
    await using var comando = CriarComando(conexao, sql);

    AdicionarParametro(comando, "@nome", jogo.Nome);
    AdicionarParametro(comando, "@descricao", jogo.Descricao);
    AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
    AdicionarParametro(comando, "@preco", jogo.Preco);
    AdicionarParametro(comando, "@estoque", jogo.Estoque);
    AdicionarParametro(comando, "@ativo", jogo.Ativo);
    AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
    AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);

    await ExecutarGravacaoAsync(comando);

    return Convert.ToInt32(comando.LastInsertedId);
}

    public async Task AlterarAsync(Jogo jogo)
{
    const string sql = """
        UPDATE tb_jogo
        SET
            no_jogo = @nome,
            ds_jogo = @descricao,
            nu_ano_lancamento = @anoLancamento,
            vl_preco = @preco,
            qt_estoque = @estoque,
            co_plataforma = @idPlataforma,
            co_condicao_jogo = @idCondicaoJogo
        WHERE co_seq_jogo = @id;
        """;

    await using var conexao = await AbrirConexaoAsync();
    await using var comando = CriarComando(conexao, sql);

    AdicionarParametro(comando, "@nome", jogo.Nome);
    AdicionarParametro(comando, "@descricao", jogo.Descricao);
    AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
    AdicionarParametro(comando, "@preco", jogo.Preco);
    AdicionarParametro(comando, "@estoque", jogo.Estoque);
    AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
    AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);
    AdicionarParametro(comando, "@id", jogo.Id);

    await ExecutarGravacaoAsync(comando);
}

    public async Task DesativarAsync(int id)
{
    const string sql = """
        UPDATE tb_jogo
        SET st_ativo = FALSE
        WHERE co_seq_jogo = @id;
        """;

    await using var conexao = await AbrirConexaoAsync();
    await using var comando = CriarComando(conexao, sql);

    AdicionarParametro(comando, "@id", id);

    await ExecutarGravacaoAsync(comando);
}
}