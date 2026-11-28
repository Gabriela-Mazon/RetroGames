namespace RetroGames.WebApi.ApiModels;

public class CadastrarUsuarioRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
