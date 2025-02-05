using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace ShadowTalk.Server
{
    internal partial class Server
    {
        private User? Login(Socket socket,
                           Aes aes,
                           ICryptoTransform encryptor,
                           ICryptoTransform decryptor,
                           JObject json,
                           string key)
        {
            //TODO пользователь хэширует( пароль + ключ ) а сервер проверяет
            //TODO шифровать ли устройство пользователя и проверять????
            return null;
        }
    }
}
