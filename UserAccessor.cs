namespace ShadowTalk.Server
{
    internal class UserAccessor
    {
        public UserAccessor()
        {
            //TODO Загружать из файла/бд
        }


        private Random random = new Random();
        private short Tag()
        {
            return (short)random.Next(0, 10000);
        }
        private short RandTag()
        {
            short i = (short)random.Next(1, 100);
            if(i % 2 == 0)
            {
                i -= 1;
            }
            if (i == 0)
            {
                i = 99;
            }
            if(i % 5 == 0)
            {
                i -= 2;
            }
            return i;
        }
        private Dictionary<string, User> userName = new Dictionary<string, User>();
        private Dictionary<string, User> userUUID = new Dictionary<string, User>();

        public User? GetByName(string name)
        {
            return userName.GetValueOrDefault(name, null);
        }
        public User? GetByUUID(string UUID)
        {
            return userUUID.GetValueOrDefault(UUID, null);
        }

        public void Disconect(string UUID)
        {
            User? user = GetByUUID(UUID);
            if (user != null)
            {
                user.UUID = null;
                userUUID.Remove(UUID);
            }
        }
        public void Connect(string name, string UUID)
        {
            User? user = GetByName(name);
            if (user != null && user.UUID == null)
            {
                user.UUID = UUID;
                userUUID.Add(UUID, user);
            }
        }






        public User Add(string name)
        {
            short tag = Tag();
            short randTag = RandTag();
            int i = 0;
            for (i = 0; i < 100; i++)
            {
                if(userName.ContainsKey(SplitName(name, tag)))
                {
                    tag += randTag;
                    if(tag >= 10000)
                    {
                        tag -= 10000;
                    }
                }
                else
                {
                    break;
                }
            }
            if (i == 100)
            {
                return null;
            }
            User user = new User();
            user.Name = SplitName(name, tag);
            userName.Add(user.Name, user);

            return user;
        }




        /// <summary>
        /// Соединяет имя со случайным тегом
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Tag">Тег</param>
        /// <returns>Имя в виде name#0000</returns>
        private static string SplitName(string Name, short Tag)
        {
            // Приведение тега к строке с ведущими нулями (например, 0000)
            string formattedTag = Tag.ToString("D4");

            // Возвращаем объединённую строку
            return $"{Name}#{formattedTag}";
        }
    }
}
