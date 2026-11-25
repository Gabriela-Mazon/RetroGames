using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<ICondicaoJogoRepository> _condicoes = new();
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IJogoGeneroRepository> _vinculos = new();
    private readonly Mock<IRepositorySet> _repositorio = new();
    private readonly Mock<IUsuarioAtualService> _usuarioAtual = new();

    public JogoServiceTests()
    {
        _repositorio.Setup(repositorio => repositorio.Jogo).Returns(_jogos.Object);
        _repositorio.Setup(repositorio => repositorio.Plataforma).Returns(_plataformas.Object);
        _repositorio.Setup(repositorio => repositorio.CondicaoJogo).Returns(_condicoes.Object);
        _repositorio.Setup(repositorio => repositorio.Genero).Returns(_generos.Object);
        _repositorio.Setup(repositorio => repositorio.JogoGenero).Returns(_vinculos.Object);
    }

    private JogoService CriarService()
    {
        return new JogoService(_repositorio.Object, _usuarioAtual.Object);
    }

    private static Jogo CriarJogoValido()
    {
        return new Jogo
        {
            Nome = "Super Mario World",
            AnoLancamento = 1990,
            Preco = 119.90m,
            Estoque = 3,
            Ativo = true,
            IdPlataforma = 3,
            IdCondicaoJogo = 2
        };
    }

    [Fact]
    public async Task Inserir_PlataformaInexistente_LancaNaoEncontrada()
    {
        // Arrange
        var jogo = CriarJogoValido();
        _plataformas
            .Setup(repositorio => repositorio.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync((Plataforma?)null);

        var servico = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(jogo, new List<int> { 1 }));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, excecao.Erro);
        _jogos.Verify(
            repositorio => repositorio.InserirAsync(It.IsAny<Jogo>()),
            Times.Never);
        _vinculos.Verify(
            repositorio => repositorio.InserirAsync(It.IsAny<int>(), It.IsAny<List<int>>()),
            Times.Never);
    }

    [Fact]
    public async Task Inserir_CondicaoInexistente_LancaNaoEncontrada()
    {
        // Arrange
        var jogo = CriarJogoValido();
        _plataformas
            .Setup(repositorio => repositorio.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync(new Plataforma { Id = jogo.IdPlataforma });
        _condicoes
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>
            {
                new CondicaoJogo { Id = 1, Descricao = "Novo" }
            });

        var servico = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(jogo, new List<int> { 1 }));

        // Assert
        Assert.Equal(BusinessErrors.CONDICAO_NAO_ENCONTRADA, excecao.Erro);
        _jogos.Verify(
            repositorio => repositorio.InserirAsync(It.IsAny<Jogo>()),
            Times.Never);
        _vinculos.Verify(
            repositorio => repositorio.InserirAsync(It.IsAny<int>(), It.IsAny<List<int>>()),
            Times.Never);
    }

    [Fact]
    public async Task Inserir_GeneroInexistente_LancaNaoEncontrado()
    {
        // Arrange
        var jogo = CriarJogoValido();
        _plataformas
            .Setup(repositorio => repositorio.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync(new Plataforma { Id = jogo.IdPlataforma });
        _condicoes
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>
            {
                new CondicaoJogo { Id = jogo.IdCondicaoJogo, Descricao = "Usado" }
            });
        _generos
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(new List<Genero>
            {
                new Genero { Id = 1, Nome = "Ação" }
            });

        var servico = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(jogo, new List<int> { 1, 999 }));

        // Assert
        Assert.Equal(BusinessErrors.GENERO_NAO_ENCONTRADO, excecao.Erro);
        _jogos.Verify(
            repositorio => repositorio.InserirAsync(It.IsAny<Jogo>()),
            Times.Never);
        _vinculos.Verify(
            repositorio => repositorio.InserirAsync(It.IsAny<int>(), It.IsAny<List<int>>()),
            Times.Never);
    }

    [Fact]
    public async Task Listar_ComGenero_EnviaSomenteIdsDosJogosDoGenero()
    {
        // Arrange
        _vinculos
            .Setup(repositorio => repositorio.ListarTodosAsync())
            .ReturnsAsync(new List<JogoGenero>
            {
                new JogoGenero { IdJogo = 10, IdGenero = 1 },
                new JogoGenero { IdJogo = 10, IdGenero = 2 },
                new JogoGenero { IdJogo = 20, IdGenero = 2 },
                new JogoGenero { IdJogo = 30, IdGenero = 1 }
            });
        _jogos
            .Setup(repositorio => repositorio.ListarAsync(
                "Mario", 3, It.IsAny<List<int>?>(), 2, 5, false))
            .ReturnsAsync((new List<Jogo>(), 0));

        var servico = CriarService();

        // Act
        await servico.ListarAsync(
            busca: "Mario",
            idPlataforma: 3,
            idGenero: 2,
            pagina: 2,
            tamanhoPagina: 5);

        // Assert
        _jogos.Verify(
            repositorio => repositorio.ListarAsync(
                "Mario",
                3,
                It.Is<List<int>?>(ids => ids != null && ids.Count == 2 &&
                    ids.Contains(10) && ids.Contains(20)),
                2,
                5,
                false),
            Times.Once);
    }

    [Fact]
    public async Task Listar_SemGenero_NaoConsultaVinculosParaFiltrarNemRestringeIds()
    {
        // Arrange
        _jogos
            .Setup(repositorio => repositorio.ListarAsync(null, null, null, 1, 10, false))
            .ReturnsAsync((new List<Jogo>(), 0));

        var servico = CriarService();

        // Act
        await servico.ListarAsync();

        // Assert
        _vinculos.Verify(
            repositorio => repositorio.ListarTodosAsync(),
            Times.Never);
        _jogos.Verify(
            repositorio => repositorio.ListarAsync(null, null, null, 1, 10, false),
            Times.Once);
    }

    [Fact]
    public async Task Listar_PaginaComJogos_MontaPlataformasCondicoesEGeneros()
    {
        // Arrange
        var primeiroJogo = CriarJogoValido();
        primeiroJogo.Id = 10;

        var segundoJogo = new Jogo
        {
            Id = 20,
            Nome = "Metroid Fusion",
            AnoLancamento = 2002,
            Preco = 149.90m,
            Estoque = 1,
            Ativo = true,
            IdPlataforma = 7,
            IdCondicaoJogo = 1
        };

        _jogos
            .Setup(repositorio => repositorio.ListarAsync(null, null, null, 2, 2, false))
            .ReturnsAsync((new List<Jogo> { primeiroJogo, segundoJogo }, 5));
        _plataformas
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(new List<Plataforma>
            {
                new Plataforma { Id = 7, Nome = "Game Boy Advance" },
                new Plataforma { Id = 3, Nome = "Super Nintendo" }
            });
        _condicoes
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>
            {
                new CondicaoJogo { Id = 1, Descricao = "Novo" },
                new CondicaoJogo { Id = 2, Descricao = "Usado" }
            });
        _generos
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(new List<Genero>
            {
                new Genero { Id = 1, Nome = "Ação" },
                new Genero { Id = 2, Nome = "Aventura" },
                new Genero { Id = 3, Nome = "Plataforma" }
            });
        _vinculos
            .Setup(repositorio => repositorio.ListarPorJogosAsync(
                It.Is<List<int>>(ids => ids.Count == 2 &&
                    ids.Contains(10) && ids.Contains(20))))
            .ReturnsAsync(new List<JogoGenero>
            {
                new JogoGenero { IdJogo = 10, IdGenero = 3 },
                new JogoGenero { IdJogo = 20, IdGenero = 1 },
                new JogoGenero { IdJogo = 20, IdGenero = 2 }
            });

        var servico = CriarService();

        // Act
        var (jogos, total) = await servico.ListarAsync(pagina: 2, tamanhoPagina: 2);

        // Assert
        Assert.Equal(5, total);
        Assert.Collection(jogos,
            detalhe =>
            {
                Assert.Equal(10, detalhe.Jogo.Id);
                Assert.Equal("Super Nintendo", detalhe.NomePlataforma);
                Assert.Equal("Usado", detalhe.DescricaoCondicao);
                var genero = Assert.Single(detalhe.Generos);
                Assert.Equal(3, genero.Id);
                Assert.Equal("Plataforma", genero.Nome);
            },
            detalhe =>
            {
                Assert.Equal(20, detalhe.Jogo.Id);
                Assert.Equal("Game Boy Advance", detalhe.NomePlataforma);
                Assert.Equal("Novo", detalhe.DescricaoCondicao);
                Assert.Equal(new[] { 1, 2 }, detalhe.Generos.Select(genero => genero.Id).OrderBy(id => id));
            });
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrado()
    {
        // Arrange
        const int idInexistente = 999;
        _jogos
            .Setup(repositorio => repositorio.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((Jogo?)null);

        var servico = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.BuscarPorIdAsync(idInexistente));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, excecao.Erro);
        _jogos.Verify(
            repositorio => repositorio.BuscarPorIdAsync(idInexistente),
            Times.Once);
    }

    [Fact]
    public async Task Desativar_IdInexistente_LancaNaoEncontrado()
    {
        // Arrange
        const int idInexistente = 999;
        _jogos
            .Setup(repositorio => repositorio.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((Jogo?)null);

        var servico = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.DesativarAsync(idInexistente));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, excecao.Erro);
        _jogos.Verify(
            repositorio => repositorio.DesativarAsync(It.IsAny<int>()),
            Times.Never);
    }
}
