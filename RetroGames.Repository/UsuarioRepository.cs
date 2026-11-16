
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    public UsuarioRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        const string sql = """
            SELECT
                co_seq_usuario,
                no_usuario,
                ds_email,
                ds_senha_hash,
                st_ativo,
                dt_cadastro,
                co_perfil
            FROM tb_usuario
            WHERE ds_email = @email;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@email", email);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (!await leitor.ReadAsync())
        {
            return null;
        }

        return MapearUsuario(leitor);
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT
                co_seq_usuario,
                no_usuario,
                ds_email,
                ds_senha_hash,
                st_ativo,
                dt_cadastro,
                co_perfil
            FROM tb_usuario
            WHERE co_seq_usuario = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (!await leitor.ReadAsync())
        {
            return null;
        }

        return MapearUsuario(leitor);
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        const string sql = """
            SELECT COUNT(*) > 0
            FROM tb_usuario
            WHERE co_perfil = @idPerfil;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(
            comando,
            "@idPerfil",
            PerfilIds.Administrador);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToBoolean(resultado);
    }

    public async Task<int> InserirAsync(Usuario usuario)
    {
        const string sql = """
            INSERT INTO tb_usuario (
                no_usuario,
                ds_email,
                ds_senha_hash,
                st_ativo,
                dt_cadastro,
                co_perfil
            )
            VALUES (
                @nome,
                @email,
                @senhaHash,
                @ativo,
                @dataCadastro,
                @idPerfil
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@senhaHash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@dataCadastro", usuario.DataCadastro);
        AdicionarParametro(comando, "@idPerfil", usuario.IdPerfil);

        await ExecutarGravacaoAsync(comando);

        return Convert.ToInt32(comando.LastInsertedId);
    }

    private static Usuario MapearUsuario(
        MySqlConnector.MySqlDataReader leitor)
    {
        return new Usuario
        {
            Id = leitor.GetInt32("co_seq_usuario"),
            Nome = leitor.GetString("no_usuario"),
            Email = leitor.GetString("ds_email"),
            SenhaHash = leitor.GetString("ds_senha_hash"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPerfil = leitor.GetInt32("co_perfil")
        };
    }
}
