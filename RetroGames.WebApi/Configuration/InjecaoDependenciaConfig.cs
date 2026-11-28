using RetroGames.Repository;
using RetroGames.Services;
using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.WebApi.Services;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(
        this IServiceCollection services,
        string stringConexao)
    {
        
        services.AddSingleton(new RepositoryConfig
        {
            StringConexao = stringConexao
        });

        services.AddScoped<IPlataformaRepository, PlataformaRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IItemPedidoRepository, ItemPedidoRepository>();
        services.AddScoped<ISituacaoPedidoRepository, SituacaoPedidoRepository>();

        services.AddScoped<IRepositorySet, RepositorySet>();
        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(
        this IServiceCollection services)
    {
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IPedidoService, PedidoService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        // Permitir acesso às informações da requisição HTTP.
        services.AddHttpContextAccessor();
        // Disponibilizar o usuário atual para os Services.
        services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();
        return services;
    }
}