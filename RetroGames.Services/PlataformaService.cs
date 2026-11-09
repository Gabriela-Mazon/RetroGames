using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet _repositorio;

    public PlataformaService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        return await _repositorio.Plataforma.ListarAsync();
    }


    public async Task<Plataforma> BuscarPorIdAsync(int id)
{
    var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(id);

    if (plataforma is null)
    {
        throw new BusinessRuleException(
            BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
    }

    return plataforma;
}

    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
{
    // Remover espaços extras do início e do fim.
    plataforma.Nome = plataforma.Nome.Trim();

    // Verificar se já existe uma plataforma com esse nome.
    bool existe = await _repositorio.Plataforma
        .ExisteComNomeAsync(plataforma.Nome);

    if (existe)
    {
        throw new BusinessRuleException(
            BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
    }

    try
    {
        // Cadastrar e recuperar o ID gerado pelo banco.
        plataforma.Id = await _repositorio.Plataforma
            .InserirAsync(plataforma);

        return plataforma;
    }
    catch (RegistroDuplicadoException)
    {
        // Tratar duplicidade detectada durante a gravação.
        throw new BusinessRuleException(
            BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
    }
}

    public async Task AlterarAsync(Plataforma plataforma)
{
    // Verificar se a plataforma existe.
    var existente = await _repositorio.Plataforma
        .BuscarPorIdAsync(plataforma.Id);

    if (existente is null)
    {
        throw new BusinessRuleException(
            BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
    }

    // Remover espaços do início e do fim do nome.
    plataforma.Nome = plataforma.Nome.Trim();

    // Verificar duplicidade, ignorando a própria plataforma.
    bool existeNome = await _repositorio.Plataforma
        .ExisteComNomeAsync(plataforma.Nome, plataforma.Id);

    if (existeNome)
    {
        throw new BusinessRuleException(
            BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
    }

    try
    {
        // Atualizar a plataforma.
        await _repositorio.Plataforma.AlterarAsync(plataforma);
    }
    catch (RegistroDuplicadoException)
    {
        throw new BusinessRuleException(
            BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
    }
}

    public async Task ExcluirAsync(int id)
{
    // Verificar se a plataforma existe.
    var plataforma = await _repositorio.Plataforma
        .BuscarPorIdAsync(id);

    if (plataforma is null)
    {
        throw new BusinessRuleException(
            BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
    }

    // Verificar se existem jogos vinculados à plataforma.
    bool possuiJogos = await _repositorio.Plataforma
        .PossuiJogosAsync(id);

    if (possuiJogos)
    {
        throw new BusinessRuleException(
            BusinessErrors.PLATAFORMA_POSSUI_JOGOS);
    }

    // Excluir somente quando não houver jogos vinculados.
    await _repositorio.Plataforma.ExcluirAsync(id);
}
}