using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;
using RetroGames.Services;

// Carregar o arquivo .env.
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Ler a connection string.
var stringConexao = builder.Configuration.GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(stringConexao))
{
    throw new InvalidOperationException(
        "ConnectionStrings__Default não foi encontrada. Verifique o arquivo .env.");
}

// Registrar os Repositories e Services.
builder.Services.AddRepositorios(stringConexao);
builder.Services.AddServicosDeNegocio();

// Configurar autenticação por cookie e autorização.
builder.Services.AddAutenticacao();
builder.Services.AddAutorizacao();

// Registrar os validators do projeto da API.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Registrar os Controllers e o filtro de validação.
builder.Services
    .AddControllers(opcoes =>
    {
        opcoes.Filters.Add<FluentValidationFilter>();
    })
    .ConfigureApiBehaviorOptions(opcoes =>
    {
        // Padronizar erros de leitura e conversão dos dados recebidos.
        opcoes.InvalidModelStateResponseFactory = contexto =>
            new BadRequestObjectResult(
                new RetroGamesResponseApiModel
                {
                    ResponseCode =
                        ResponseCodes.MODEL_STATE_INVALIDO.Code,
                    ErrorMessage =
                        ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage
                });
    });

// Manter a documentação da API.
builder.Services.AddOpenApi();
// Registrar o tratamento global de exceções.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var escopo = app.Services.CreateScope())
{
    var autenticacao = escopo.ServiceProvider
        .GetRequiredService<IAutenticacaoService>();

    var email = app.Configuration["AdminInicial:Email"];
    var senha = app.Configuration["AdminInicial:Senha"];

    await autenticacao.GarantirAdministradorInicialAsync(
        email ?? string.Empty,
        senha ?? string.Empty);
}

// Tratar exceções antes das demais etapas.
app.UseExceptionHandler();

// Disponibilizar o Swagger somente em desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1");
        options.RoutePrefix = "swagger";
    });
}
// Identificar o usuário pela sessão.
app.UseAuthentication();

// Verificar as permissões do usuário.
app.UseAuthorization();

// Mapear as rotas dos Controllers.
app.MapControllers();

app.Run();