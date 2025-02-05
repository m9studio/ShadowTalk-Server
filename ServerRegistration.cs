
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace ShadowTalk.Server
{
    internal partial class Server
    {
        private User? Registration(Socket socket,
                                  Aes aes,
                                  ICryptoTransform encryptor,
                                  ICryptoTransform decryptor,
                                  JObject json,
                                  string key)
        {


            //TODO сохранять ли устройство пользователя????
            //TODO договариваемся о пароле через дифихелмана
            //TODO пользователь хэширует( пароль + ключ ) а сервер проверяет, правильный ли пароль мы получили

            return null;
        }
    }
}
