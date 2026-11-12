using RetroGames.WebApi.Configuration;

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

// Registrar Controllers e OpenAPI.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Disponibilizar o Swagger somente em desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1");
        options.RoutePrefix = "swagger";
    });
}

// Mapear as rotas dos Controllers.
app.MapControllers();

app.Run();