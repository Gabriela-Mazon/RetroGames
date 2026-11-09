using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(
        List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        // Buscar os dados necessários em quatro chamadas.
        var plataformas = await _repositorio.Plataforma.ListarAsync();
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        var generos = await _repositorio.Genero.ListarAsync();

        var idsJogos = jogos.Select(j => j.Id).ToList();

        var vinculos = await _repositorio.JogoGenero
            .ListarPorJogosAsync(idsJogos);

        // Combinar as informações usando LINQ.
        var detalhes = jogos.Select(jogo =>
        {
            var plataforma = plataformas.First(
                p => p.Id == jogo.IdPlataforma);

            var condicao = condicoes.First(
                c => c.Id == jogo.IdCondicaoJogo);

            var idsGeneros = vinculos
                .Where(v => v.IdJogo == jogo.Id)
                .Select(v => v.IdGenero)
                .ToHashSet();

            return new JogoDetalheServiceModel
            {
                Jogo = jogo,
                NomePlataforma = plataforma.Nome,
                DescricaoCondicao = condicao.Descricao,
                Generos = generos
                    .Where(g => idsGeneros.Contains(g.Id))
                    .ToList()
            };
        }).ToList();

        return detalhes;
    }


    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
    string? busca = null,
    int? idPlataforma = null,
    int? idGenero = null,
    int pagina = 1,
    int tamanhoPagina = 10,
    bool incluirInativos = false)
{
    List<int>? idsJogos = null;

    // Se foi informado um gênero, buscar os jogos vinculados a ele.
    if (idGenero.HasValue)
    {
        var vinculos = await _repositorio.JogoGenero.ListarTodosAsync();

        idsJogos = vinculos
            .Where(v => v.IdGenero == idGenero.Value)
            .Select(v => v.IdJogo)
            .Distinct()
            .ToList();

        // Nenhum jogo pertence ao gênero informado.
        if (idsJogos.Count == 0)
        {
            return (new List<JogoDetalheServiceModel>(), 0);
        }
    }

    // Buscar os jogos aplicando filtros e paginação no banco.
    var (jogos, total) = await _repositorio.Jogo.ListarAsync(
        busca: busca,
        idPlataforma: idPlataforma,
        ids: idsJogos,
        pagina: pagina,
        tamanhoPagina: tamanhoPagina,
        incluirInativos: incluirInativos
    );

    // Combinar jogos, plataformas, condições e gêneros.
    var detalhes = await MontarDetalhesAsync(jogos);

    return (detalhes, total);
}

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
{
    // Buscar o jogo pelo identificador.
    var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

    // Se não existir, lançar o erro de negócio.
    if (jogo is null)
    {
        throw new BusinessRuleException(
            BusinessErrors.JOGO_NAO_ENCONTRADO);
    }

    // Montar o detalhe com plataforma, condição e gêneros.
    var detalhes = await MontarDetalhesAsync(
        new List<Jogo> { jogo });

    return detalhes[0];
}

    public async Task<Jogo> InserirAsync(Jogo jogo, List<int> idsGeneros)
{
    // Remover espaços do nome e marcar o jogo como ativo.
    jogo.Nome = jogo.Nome.Trim();
    jogo.Ativo = true;

    // Verificar se a plataforma existe.
    var plataforma = await _repositorio.Plataforma
        .BuscarPorIdAsync(jogo.IdPlataforma);

    if (plataforma is null)
    {
        throw new BusinessRuleException(
            BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
    }

    // Verificar se a condição existe.
    var condicoes = await _repositorio.CondicaoJogo.ListarAsync();

    if (!condicoes.Any(c => c.Id == jogo.IdCondicaoJogo))
    {
        throw new BusinessRuleException(
            BusinessErrors.CONDICAO_NAO_ENCONTRADA);
    }

    // Verificar os gêneros informados.
    var generos = await _repositorio.Genero.ListarAsync();

    var idsValidos = generos
        .Select(g => g.Id)
        .ToHashSet();

    var idsInformados = idsGeneros.Distinct().ToList();

    if (idsInformados.Count == 0 ||
        idsInformados.Any(id => !idsValidos.Contains(id)))
    {
        throw new BusinessRuleException(
            BusinessErrors.GENERO_NAO_ENCONTRADO);
    }

    // Cadastrar o jogo e recuperar o ID gerado.
    jogo.Id = await _repositorio.Jogo.InserirAsync(jogo);

    // Associar o jogo aos gêneros informados.
    await _repositorio.JogoGenero.InserirAsync(
        jogo.Id, idsInformados);

    return jogo;
}

    public async Task AlterarAsync(Jogo jogo, List<int> idsGeneros)
{
    // Verificar se o jogo existe.
    var existente = await _repositorio.Jogo.BuscarPorIdAsync(jogo.Id);

    if (existente is null)
    {
        throw new BusinessRuleException(
            BusinessErrors.JOGO_NAO_ENCONTRADO);
    }

    // Remover espaços do nome.
    jogo.Nome = jogo.Nome.Trim();

    // Verificar se a plataforma existe.
    var plataforma = await _repositorio.Plataforma
        .BuscarPorIdAsync(jogo.IdPlataforma);

    if (plataforma is null)
    {
        throw new BusinessRuleException(
            BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
    }

    // Verificar se a condição existe.
    var condicoes = await _repositorio.CondicaoJogo.ListarAsync();

    if (!condicoes.Any(c => c.Id == jogo.IdCondicaoJogo))
    {
        throw new BusinessRuleException(
            BusinessErrors.CONDICAO_NAO_ENCONTRADA);
    }

    // Verificar se todos os gêneros existem.
    var generos = await _repositorio.Genero.ListarAsync();

    var idsValidos = generos
        .Select(g => g.Id)
        .ToHashSet();

    var idsInformados = idsGeneros.Distinct().ToList();

    if (idsInformados.Count == 0 ||
        idsInformados.Any(id => !idsValidos.Contains(id)))
    {
        throw new BusinessRuleException(
            BusinessErrors.GENERO_NAO_ENCONTRADO);
    }

    // Atualizar os dados do jogo.
    await _repositorio.Jogo.AlterarAsync(jogo);

    // Remover os vínculos antigos.
    await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);

    // Cadastrar os novos vínculos.
    await _repositorio.JogoGenero.InserirAsync(
        jogo.Id, idsInformados);
}

    public async Task DesativarAsync(int id)
{
    // Verificar se o jogo existe.
    var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

    if (jogo is null)
    {
        throw new BusinessRuleException(
            BusinessErrors.JOGO_NAO_ENCONTRADO);
    }

    // Marcar o jogo como inativo.
    await _repositorio.Jogo.DesativarAsync(id);
}
}