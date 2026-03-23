namespace TurismoRural_API.Services
{
    public interface IPasswordHelper
    {
        string Encrypt(string texto);
        void EnviarCorreo(string destinatario, string asunto, string contenido);
    }
}