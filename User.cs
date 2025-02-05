using System.Security.Cryptography;

namespace ShadowTalk.Server
{
    internal class User
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public string? UUID { get; set; }


        public Aes Aes { get; set; }
        public ICryptoTransform Encryptor { get; set; }
        public ICryptoTransform Decryptor { get; set; }


        /// <summary>
        /// Устройство
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }


    }
}
