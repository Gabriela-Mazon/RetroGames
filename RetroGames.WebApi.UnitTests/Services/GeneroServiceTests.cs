using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
    [Fact]
    public async Task Listar_RepositorioComGeneros_DevolveGenerosRecebidos()
    {
        // Arrange
        var generos = new List<Genero>
        {
            new Genero { Id = 1, Nome = "Ação" },
            new Genero { Id = 2, Nome = "Aventura" }
        };

        var repositorioGeneros = new Mock<IGeneroRepository>();
        repositorioGeneros
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(generos);

        var repositorio = new Mock<IRepositorySet>();
        repositorio
            .Setup(conjunto => conjunto.Genero)
            .Returns(repositorioGeneros.Object);

        var servico = new GeneroService(repositorio.Object);

        // Act
        var resultado = await servico.ListarAsync();

        // Assert
        Assert.Collection(resultado,
            genero =>
            {
                Assert.Equal(1, genero.Id);
                Assert.Equal("Ação", genero.Nome);
            },
            genero =>
            {
                Assert.Equal(2, genero.Id);
                Assert.Equal("Aventura", genero.Nome);
            });
        repositorioGeneros.Verify(repositorio => repositorio.ListarAsync(), Times.Once);
    }
}
