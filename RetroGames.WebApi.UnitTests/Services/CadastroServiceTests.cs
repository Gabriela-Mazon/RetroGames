using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class CadastroServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly PasswordHasher<Usuario> _hasher = new();
    private readonly AutenticacaoService _service;

    public CadastroServiceTests()
    {
        var repos = new Mock<IRepositorySet>();
        repos.SetupGet(x => x.Usuario).Returns(_usuarios.Object);
        _service = new AutenticacaoService(repos.Object, _hasher);
    }

    [Fact]
    public async Task CadastroValido_CriaClienteAtivoComHashVerificavel()
    {
        _usuarios.Setup(x => x.InserirAsync(It.IsAny<Usuario>())).ReturnsAsync(7);
        var senha = Guid.NewGuid().ToString();
        var usuario = await _service.CadastrarAsync("  Ana  ", " ana@example.test ", senha);

        Assert.Equal(7, usuario.Id);
        Assert.Equal("Ana", usuario.Nome);
        Assert.Equal("ana@example.test", usuario.Email);
        Assert.True(usuario.Ativo);
        Assert.Equal(PerfilIds.Cliente, usuario.IdPerfil);
        Assert.Equal(DateTimeKind.Utc, usuario.DataCadastro.Kind);
        Assert.NotEqual(senha, usuario.SenhaHash);
        Assert.Equal(PasswordVerificationResult.Success,
            _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha));
        _usuarios.Verify(x => x.InserirAsync(usuario), Times.Once);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EmailDuplicado_InclusiveEmCadastrosSimultaneos_RetornaErro106(bool concorrente)
    {
        const string email = "ana@example.test";
        if (concorrente)
            _usuarios.Setup(x => x.InserirAsync(It.IsAny<Usuario>()))
                .ThrowsAsync(new RegistroDuplicadoException("Duplicado", new Exception()));
        else
            _usuarios.Setup(x => x.BuscarPorEmailAsync(email)).ReturnsAsync(new Usuario());

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _service.CadastrarAsync("Ana", email, Guid.NewGuid().ToString()));
        Assert.Equal(BusinessErrors.EMAIL_JA_CADASTRADO, erro.Erro);
        _usuarios.Verify(x => x.InserirAsync(It.IsAny<Usuario>()),
            concorrente ? Times.Once() : Times.Never());
    }
}
