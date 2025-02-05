using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

namespace ShadowTalk.Server
{
    internal partial class Server
    {
        public bool IsOpen { get { return _socket != null && _socket.Connected; } }

        private protected Socket _socket;
        private IPEndPoint _ipEndPoint;

        // Хранение подключённых пользователей
        private Dictionary<string, Socket> _clients;
        private UserAccessor _users { get; } = new UserAccessor();

        public Server() : this("127.0.0.1", 10101) { }
        public Server(string host, ushort port)
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public bool Open()
        {
            Console.WriteLine("Запуск сервера...");
            try
            {
                _socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                _socket.Bind(_ipEndPoint);
                _socket.Listen(10);

                Console.WriteLine($"Сервер запущен на {_ipEndPoint.Address}:{_ipEndPoint.Port}");
                Task.Run(AcceptClientsAsync); // Асинхронно принимаем подключения
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка запуска сервера: {ex.Message}");
            }
            return IsOpen;
        }

        public bool Close()
        {
            if (IsOpen && _socket != null)
            {
                try
                {
                    foreach (var client in _clients.Values)
                    {
                        client.Close(); // Закрытие всех клиентских сокетов
                    }
                    _clients.Clear();

                    _socket.Close();
                    Console.WriteLine("Сервер остановлен.");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при остановке сервера: {ex.Message}");
                }
            }
            return false;
        }



        private async Task AcceptClientsAsync()
        {
            while (IsOpen && _socket != null)
            {
                try
                {
                    Socket clientSocket = await _socket.AcceptAsync();
                    User? user = Verification(clientSocket);
                    if (user != null)
                    {
                        string clientId = Guid.NewGuid().ToString();
                        _clients.Add(clientId, clientSocket);
                        _users.Connect(user.Name, clientId);
                        Console.WriteLine($"Новый клиент подключился: {clientId}");
                        _ = HandleClientAsync(clientSocket);//TODO что делает _ ?
                    }
                    else
                    {
                        clientSocket.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при принятии клиента: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(/*string clientId,*/ Socket clientSocket)
        {
            var buffer = new byte[1024];

            try
            {
                while (true)
                {



                    int bytesRead = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                    if (bytesRead == 0) break; // Клиент отключился

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    //Console.WriteLine($"[{clientId}] Получено сообщение: {message}");

                    // Обработка сообщения (будущая логика)
                    //BroadcastMessage($"[{clientId}] сказал: {message}");
                }
            }
            catch (Exception ex)
            {
               // Console.WriteLine($"Ошибка при обработке клиента {clientId}: {ex.Message}");
            }
            finally
            {
                //Console.WriteLine($"Клиент отключился: {clientId}");
                //clients.Remove(clientId);
                clientSocket.Close();

                // Уведомление об отключении клиента
                //BroadcastMessage($"Пользователь {clientId} отключился.");
            }
        }

        // Отправка сообщения конкретному пользователю
        public void Send(string userId, string msg)
        {
            if (_clients.TryGetValue(userId, out var clientSocket))
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    clientSocket.Send(data);
                    Console.WriteLine($"Сообщение отправлено пользователю {userId}: {msg}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке сообщения пользователю {userId}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Пользователь {userId} не найден.");
            }
        }

        // Широковещательная отправка сообщений всем клиентам
        private void BroadcastMessage(string msg)
        {
            foreach (var clientId in _clients.Keys.ToList())
            {
                Send(clientId, msg);
            }
        }
    
    
    
    
    
    
    
    
        private static byte[] GetMessage(Socket socket)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = socket.Receive(buffer)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                    if (bytesRead < 1024) break; // Если получили меньше 1024 байт, выходим из цикла
                }
                return ms.ToArray();
            }
        }
    
    
    }
}
