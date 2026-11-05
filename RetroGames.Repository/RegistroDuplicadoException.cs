namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(string mensagem, Exception excecaoOriginal)
        : base(mensagem, excecaoOriginal)
    {
    }
}