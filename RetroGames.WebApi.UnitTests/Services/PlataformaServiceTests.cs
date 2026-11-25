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
}
