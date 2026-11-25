using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<IRepositorySet> _repositorio = new();

    public PlataformaServiceTests()
    {
        _repositorio
            .Setup(repositorio => repositorio.Plataforma)
            .Returns(_plataformas.Object);
    }

    private PlataformaService CriarService()
    {
        return new PlataformaService(_repositorio.Object);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        const int idInexistente = 999;
        _plataformas
            .Setup(repositorio => repositorio.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((Plataforma?)null);

        var servico = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.BuscarPorIdAsync(idInexistente));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, excecao.Erro);
        _plataformas.Verify(
            repositorio => repositorio.BuscarPorIdAsync(idInexistente),
            Times.Once);
    }

    [Fact]
    public async Task Inserir_NomeJaCadastrado_LancaNomeDuplicado()
    {
        // Arrange
        var plataforma = new Plataforma { Nome = "Super Nintendo" };
        _plataformas
            .Setup(repositorio => repositorio.ExisteComNomeAsync(plataforma.Nome, null))
            .ReturnsAsync(true);

        var servico = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(plataforma));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, excecao.Erro);
        _plataformas.Verify(
            repositorio => repositorio.InserirAsync(It.IsAny<Plataforma>()),
            Times.Never);
    }

    [Fact]
    public async Task Inserir_RepositoryDetectaDuplicidade_LancaNomeDuplicado()
    {
        // Arrange
        var plataforma = new Plataforma { Nome = "Super Nintendo" };
        _plataformas
            .Setup(repositorio => repositorio.ExisteComNomeAsync(plataforma.Nome, null))
            .ReturnsAsync(false);
        _plataformas
            .Setup(repositorio => repositorio.InserirAsync(plataforma))
            .ThrowsAsync(new RegistroDuplicadoException(
                "Nome de plataforma duplicado.",
                new InvalidOperationException("Falha de unicidade simulada.")));

        var servico = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(plataforma));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, excecao.Erro);
        _plataformas.Verify(
            repositorio => repositorio.InserirAsync(plataforma),
            Times.Once);
    }

    [Fact]
    public async Task Inserir_NomeLivre_DevolvePlataformaComIdGerado()
    {
        // Arrange
        var plataforma = new Plataforma { Nome = "Super Nintendo" };
        const int idGerado = 7;
        _plataformas
            .Setup(repositorio => repositorio.ExisteComNomeAsync(plataforma.Nome, null))
            .ReturnsAsync(false);
        _plataformas
            .Setup(repositorio => repositorio.InserirAsync(plataforma))
            .ReturnsAsync(idGerado);

        var servico = CriarService();

        // Act
        var resultado = await servico.InserirAsync(plataforma);

        // Assert
        Assert.Equal(idGerado, resultado.Id);
        Assert.Equal("Super Nintendo", resultado.Nome);
        _plataformas.Verify(
            repositorio => repositorio.InserirAsync(plataforma),
            Times.Once);
    }

    [Fact]
    public async Task Alterar_MantemProprioNome_GravaAlteracao()
    {
        // Arrange
        var plataforma = new Plataforma { Id = 7, Nome = "Super Nintendo" };
        _plataformas
            .Setup(repositorio => repositorio.BuscarPorIdAsync(plataforma.Id))
            .ReturnsAsync(plataforma);
        _plataformas
            .Setup(repositorio => repositorio.ExisteComNomeAsync(plataforma.Nome, plataforma.Id))
            .ReturnsAsync(false);
        _plataformas
            .Setup(repositorio => repositorio.AlterarAsync(plataforma))
            .Returns(Task.CompletedTask);

        var servico = CriarService();

        // Act
        await servico.AlterarAsync(plataforma);

        // Assert
        _plataformas.Verify(
            repositorio => repositorio.ExisteComNomeAsync(plataforma.Nome, plataforma.Id),
            Times.Once);
        _plataformas.Verify(
            repositorio => repositorio.AlterarAsync(plataforma),
            Times.Once);
    }

    [Fact]
    public async Task Excluir_PlataformaComJogos_LancaPossuiJogos()
    {
        // Arrange
        var plataforma = new Plataforma { Id = 7, Nome = "Super Nintendo" };
        _plataformas
            .Setup(repositorio => repositorio.BuscarPorIdAsync(plataforma.Id))
            .ReturnsAsync(plataforma);
        _plataformas
            .Setup(repositorio => repositorio.PossuiJogosAsync(plataforma.Id))
            .ReturnsAsync(true);

        var servico = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.ExcluirAsync(plataforma.Id));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_POSSUI_JOGOS, excecao.Erro);
        _plataformas.Verify(
            repositorio => repositorio.ExcluirAsync(It.IsAny<int>()),
            Times.Never);
    }
}
