using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace My_server
{
    public class server
    {
        private static int connectedClients = 0;
        private static object clientLock = new object();

        public static void Main(string[] args)
        {
            // IP-адрес и порт сервера
            string ipAddress = new Config().Ip;
            int port = new Config().Port;
            // создание сервера
            TcpListener server = new TcpListener(IPAddress.Parse(ipAddress), port);
            server.Start();
            Console.WriteLine(server.LocalEndpoint);
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                // ожидание подключения клиента
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Клиент подключился!");

                lock (clientLock)
                {
                    if (connectedClients >= 4)
                    {
                        Console.WriteLine("Достигнуто максимальное число подключений.");
                        client.Close();
                        continue;
                    }

                    connectedClients++;
                    PrintConnectedClientsCount();
                }

                Thread clientThread = new Thread(() => HandleClientRequests(client));
                clientThread.Start();
            }
        }

        private static void HandleClientRequests(TcpClient client)
        {
            try
            {
                // вывод сообщения о подключении клиента
                Console.WriteLine($"Клиент {client.Client.RemoteEndPoint} подключился.");
                PrintConnectedClientsCount();

                // обработка запросов клиента
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                My_server.Methods.HandleClient(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке запросов клиента {client.Client.RemoteEndPoint}: {ex.Message}");
            }
            finally
            {
                lock (clientLock)
                {
                    connectedClients--;
                    PrintConnectedClientsCount();
                }
                client.Close();
            }
        }

        private static void PrintConnectedClientsCount()
        {
            Console.WriteLine($"Подключено клиентов: {connectedClients}");
        }
    }
}
