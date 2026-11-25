using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    [Fact]
    public async Task Listar_RepositorioComCondicoes_DevolveCondicoesRecebidas()
    {
        // Arrange
        var condicoes = new List<CondicaoJogo>
        {
            new CondicaoJogo { Id = 1, Descricao = "Novo" },
            new CondicaoJogo { Id = 2, Descricao = "Usado" }
        };

        var repositorioCondicoes = new Mock<ICondicaoJogoRepository>();
        repositorioCondicoes
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(condicoes);

        var repositorio = new Mock<IRepositorySet>();
        repositorio
            .Setup(conjunto => conjunto.CondicaoJogo)
            .Returns(repositorioCondicoes.Object);

        var servico = new CondicaoJogoService(repositorio.Object);

        // Act
        var resultado = await servico.ListarAsync();

        // Assert
        Assert.Collection(resultado,
            condicao =>
            {
                Assert.Equal(1, condicao.Id);
                Assert.Equal("Novo", condicao.Descricao);
            },
            condicao =>
            {
                Assert.Equal(2, condicao.Id);
                Assert.Equal("Usado", condicao.Descricao);
            });
        repositorioCondicoes.Verify(repositorio => repositorio.ListarAsync(), Times.Once);
    }
}
