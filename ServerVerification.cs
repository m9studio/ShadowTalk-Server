using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ShadowTalk.Server
{
    internal partial class Server
    {
        private Random Random = new Random();
        private User? Verification(Socket socket)
        {
            //Генерируем и отправляем асиметричный ключ
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            RSAParameters publicKeyParams = rsa.ExportParameters(false);
            socket.Send(publicKeyParams.Modulus);


            //Получаем симетричный ключ от клиента
            byte[] buffer = GetMessage(socket);
            if (buffer.Length == 0) return null;
            string[] keys = Encoding.UTF8
                                    .GetString(buffer, 0, buffer.Length)
                                    .Split(":");
            if (keys.Length != 2) return null;
            Aes aes = Aes.Create();
            aes.Key = rsa.Decrypt(Encoding.UTF8.GetBytes(keys[0]), false);
            aes.IV = rsa.Decrypt(Encoding.UTF8.GetBytes(keys[1]), false);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform encryptor = aes.CreateEncryptor();
            ICryptoTransform decryptor = aes.CreateDecryptor();


            string key = "";
            for (int i = 0; i < 16; i++) key += Random.Next(0, 16).ToString("X");

            //Отправляем приветствие
            byte[] data = Encoding.UTF8.GetBytes($"{{result:\"Ok\", key:\"{key}\"}}");
            socket.Send(encryptor.TransformFinalBlock(data, 0, data.Length));

            //TODO может сразу пользователь будет отправлять несколько сообщений???? без ответа сервера или может будем принимать JSON???

            //Получаем то, что хочет сделать пользователь
            buffer = GetMessage(socket);
            if (buffer.Length == 0) return null;

            JObject json = new JObject(Encoding.UTF8
                                               .GetString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length)));

            //Разделяем логику
            switch (json.GetValue("action")?.ToString())
            {
                case "registration":
                    return Registration(socket, aes, encryptor, decryptor, json, key);
                case "login":
                    return Login(socket, aes, encryptor, decryptor, json, key);
            }
            return null;


            if (json.GetValue("action")?.ToString() == "registration")
            {
                Registration(socket, aes, encryptor, decryptor, json);
            }
            else if(json.GetValue("action")?.ToString() == "login")
            {
                Login(socket, aes, encryptor, decryptor, json);
            }
            return null;



            //Генерируем секретное слово, шифруем его по симетручному ключу и отправляем его пользователю


            //Получем имя + secretKey + пароль, зашифрованные по симетручному ключу
            //TODO ожидание сообщения
            buffer = GetMessage(socket);
            if (buffer.Length == 0) return null;
            string[] result = Encoding.UTF8
                                      .GetString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length))
                                      .Split(secretKey);
            if (result.Length != 2) return null;
            string name = result[0];
            string password = result[1];
            User? user = _users.GetByName(name);
            if (user != null && user.Password == password)
            {
                user.Aes = aes;
                return user;
            }
            return null;
        }

    }
}
