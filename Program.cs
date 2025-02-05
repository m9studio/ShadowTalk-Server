namespace ShadowTalk.Server
{
    internal class Program
    {
        public static Server Server { get; } = new Server();
        static void Main(string[] args)
        {
            Server.Open();

            while (true)
            {
                string command = Console.ReadLine();
                if (command == "close")
                {
                    Server.Close();
                    break;
                }
            }
        }



    }

    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
    }

}
