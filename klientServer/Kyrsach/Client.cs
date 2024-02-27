using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
namespace Kyrsach
{
    internal class Client
    {
        private static TcpClient client;
        private static NetworkStream stream;

        static void Main(string[] args)
        {
            
          
            // IP-адрес и порт сервера
            Console.WriteLine("Введите ip адресс сервера для подключения ");
            string ipAddress = "127.0.0.1";
            Console.WriteLine("Введите порт адресс сервера для подключения ");
            int port = 8888;

            // меню запросов клиента
            while (true)
            {
            // подключение к серверу
            client = new TcpClient(ipAddress, port);
                stream = client.GetStream();

                // создаем новый объект StreamReader
                StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
                StreamReader streamReader = new StreamReader(stream);

                Console.WriteLine("Выберите опцию:");
                Console.WriteLine("1. Переместить файл с компьютера на сервер");
                Console.WriteLine("4. Выбрать файл  и создать архив с этим файлом ");
                Console.WriteLine("5. переместить файл с сервера на пк");
                Console.WriteLine("6. разорхевировать файлы на сервере или пк");
                Console.WriteLine("7. Выход");

                // отправка запроса на сервер
                string request = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes(request);
                stream.Write(buffer, 0, buffer.Length);

                if (request == "1")
                {
                    using (var uploadStream = client.GetStream())
                        {
                        Kyrsach.Methods.CopyFileFromComputerToServer(client, stream);
                    }
                }
                else if (request == "4")
                {
                    using (var uploadStream = client.GetStream())
                    {
                        Kyrsach.Methods.Arhive(client, stream);
                        }
                }
                else if (request == "5")
                {
                    Kyrsach.Methods.MoveServerPC(client, stream);
                }
                else if (request == "6")
                {
                    Kyrsach.Methods.ExtractArchive(client, stream);
                }
                else if (request == "7")
                {
                    
                    Environment.Exit(0);
                }
            }
        }
    }
}

