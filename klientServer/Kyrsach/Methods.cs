using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kyrsach
{
    public  class Methods
    {
        // отправка запроса на сервер
        public static void CopyFileFromComputerToServer(TcpClient client, NetworkStream stream)
        {
            // Отправляем запрос на копирование файла с компьютера на сервер
            Console.WriteLine("Введите путь к папке с нужными файлами");
            string folderPath = Console.ReadLine(); // Путь к папке с файлом на компьютере
                                                    // Показываем список файлов в папке
            Console.WriteLine($"Список файлов в папке {folderPath}:");
            string[] files = Directory.GetFiles(folderPath);
            foreach (string file in files)
            {
                string fName = Path.GetFileName(file);
                Console.WriteLine(fName);
            }
            string request = $"Copy computer to server{folderPath}";
            byte[] requestBytes = Encoding.UTF8.GetBytes(request);
            stream.Write(requestBytes, 0, requestBytes.Length);

            // Запрашиваем у пользователя название файла
            Console.Write("Введите название файла для копирования: ");
            string fileName = Console.ReadLine();

            // Отправляем название файла на сервер
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            stream.Write(fileNameBytes, 0, fileNameBytes.Length);

            // Получаем размер файла, который нужно скопировать на сервер
            byte[] fileSizeBytes = new byte[8];
            int bytesRead = stream.Read(fileSizeBytes, 0, fileSizeBytes.Length);
            long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

            // Создаем файл на сервере
            string savePath = $@"C:\Users\Ray_Gek\Desktop\Kyrsach\My_server\Server\{fileName}";
            using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
            {
                // Получаем данные файла и записываем их в созданный файл на сервере
               byte[] buffer = new byte[1024];
                int bytesReceived = 0;
                while (bytesReceived < fileSize)
                {
                    int bytesReadThisTime = stream.Read(buffer, 0, buffer.Length);
                    fileStream.Write(buffer, 0, bytesReadThisTime);
                    bytesReceived += bytesReadThisTime;
                }
            }

            Console.WriteLine($"Файл {fileName} успешно скопирован на сервер.");

            // Закрываем соединение с сервером
            stream.Close();
            client.Close();
        }

        public static void GetFilesListFromServer(TcpClient client, NetworkStream stream)
        {
            // отправка запроса на получение списка файлов
            string request = "list C:\\Users\\Ray_Gek\\Desktop\\Kyrsach\\My_server\\Server";
            byte[] buffer = Encoding.UTF8.GetBytes(request);
            stream.Write(buffer, 0, buffer.Length);

            // чтение ответа от сервера
            buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Список файлов в директории:");
            string[] fileNames = response.Split('\n');
            foreach (string fileName in fileNames)
            {
                Console.WriteLine(Path.GetFileName(fileName));
            }

            Console.ReadKey();
            stream.Close();
        }



        public static void DeleteFilesServer(TcpClient client, NetworkStream stream)
        {
            // отправка запроса на получение списка файлов
           string request = "delete";
           byte[] buffer = Encoding.UTF8.GetBytes(request);
            stream.Write(buffer, 0, buffer.Length);

            // чтение списка файлов от сервера
            buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Список файлов в директории:");
            string[] fileNames = response.Split('\n');
            foreach (string fileName in fileNames)
            {
                Console.WriteLine(Path.GetFileName(fileName));
            }

            // выбор файла для удаления
            Console.WriteLine("Выберите номер файла, который хотите удалить:");
            int fileNumber;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out fileNumber))
                {
                    if (fileNumber >= 1 && fileNumber <= fileNames.Length)
                    {
                        break;
                    }
                }
                Console.WriteLine("Некорректный ввод. Попробуйте еще раз.");
            }
            string selectedFile = fileNames[fileNumber - 1];

            // проверка существования файла
            string filePath = Path.Combine(@"C:\Users\Ray_Gek\Desktop\Kyrsach", selectedFile);
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл {selectedFile} не найден в директории.");
                return;
            }

            // отправка запроса на удаление файла
            request = selectedFile;
            buffer = Encoding.UTF8.GetBytes(request);
            stream.Write(buffer, 0, buffer.Length);

            // чтение ответа от сервера
            buffer = new byte[1024];
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // вывод ответа сервера на экран
            Console.WriteLine(response);
        }

       public static void Arhive(TcpClient client, NetworkStream stream)
        {
            // send request to server to get list of files for archiving
            string archiveRequest = "archive";
           byte[] buffer = Encoding.UTF8.GetBytes(archiveRequest);
            stream.Write(buffer, 0, buffer.Length);
            // получаем путь к директории от пользователя
            Console.WriteLine("Введите путь к директории:");
            string directoryPath = Console.ReadLine();
            buffer = Encoding.UTF8.GetBytes(directoryPath);
            stream.Write(buffer, 0, buffer.Length);
            // receive list of files from server
            buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string fileList = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

            // print list of files to console for user to select
            Console.WriteLine("Выберите файлы для архивирования (через запятую):");
            string[] fileNames = fileList.Split('\n');
            for (int i = 0; i < fileNames.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {fileNames[i]}");
            }
            Console.Write("Введите номера файлов для архивирования: ");
            string[] fileSelections = Console.ReadLine().Split(',');
            string selectedFileNames = string.Join(";", fileSelections.Select(selection => fileNames[int.Parse(selection) - 1]));

            // prompt user for archive name, password, and compression format
            Console.Write("Введите имя архива: ");
            string archiveName = Console.ReadLine();
            Console.Write("Введите пароль для архива: ");
            string password = Console.ReadLine();
            Console.Write("Выберите формат сжатия (rar или 7z): ");
            string compressionFormat = Console.ReadLine();

            // send file selection, archive name, password, and compression format to server
            string archiveSelection = $"{selectedFileNames},{archiveName},{password},{compressionFormat}";
            buffer = Encoding.UTF8.GetBytes(archiveSelection);
            stream.Write(buffer, 0, buffer.Length);

            // receive result of archiving operation from server and print to console
            buffer = new byte[1024];
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            Console.WriteLine(response);

        }

        public static void MoveServerPC(TcpClient client,NetworkStream stream)
        {
            // Отправляем запрос на копирование файлов с сервера на компьютер
            Console.WriteLine("Введите путь к папке, куда нужно скопировать файлы:");
            string destinationFolder = Console.ReadLine(); // Путь к папке на компьютере
            string serverFolderPath = @"C:\Users\Ray_Gek\Desktop\Kyrsach\My_server\Server"; // Путь к папке на сервере
                                                                                            // Запрашиваем у сервера список имен файлов в папке
            string request = $"Copy server to computer{serverFolderPath}";
            byte[] requestBytes = Encoding.UTF8.GetBytes(request);
            stream.Write(requestBytes, 0, requestBytes.Length);
            // Получаем список имен файлов от сервера
            byte[] fileNamesBytes = new byte[1024];
            int fileNamesBytesLength = stream.Read(fileNamesBytes, 0, fileNamesBytes.Length);
            string[] fileNames = Encoding.UTF8.GetString(fileNamesBytes, 0, fileNamesBytesLength).Split(',');
            // Показываем список файлов на сервере
            Console.WriteLine($"Список файлов в папке {serverFolderPath}:");
            foreach (string fileNam in fileNames)
            {
                string fName = Path.GetFileName(fileNam);
                Console.WriteLine(fName);
            }
            // Запрашиваем у пользователя выбранные файлы
            Console.WriteLine("Введите имена файлов, которые нужно скопировать (разделенные запятой):");
            string selectedFilesString = Console.ReadLine();
            string[] selectedFiles = selectedFilesString.Split(',');
            // Отправляем выбранные файлы на сервер
            foreach (string fileName in selectedFiles)
            {
                // Отправляем название файла на сервер
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName.Trim());
                stream.Write(fileNameBytes, 0, fileNameBytes.Length);
                // Получаем размер файла, который нужно скопировать на компьютер
                byte[] fileSizeBytes = new byte[8];
                int bytesRead = stream.Read(fileSizeBytes, 0, fileSizeBytes.Length);
                long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);
                // Создаем файл на компьютере
                string savePath = Path.Combine(destinationFolder, fileName.Trim());
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                {
                    // Получаем данные файла и записываем их в созданный файл на компьютере
                   byte[] buffer = new byte[1024];
                    int bytesReceived = 0;
                    while (bytesReceived < fileSize)
                    {
                        int bytesReadThisTime = stream.Read(buffer, 0, buffer.Length);
                        fileStream.Write(buffer, 0, bytesReadThisTime);
                        // Дополняем код, чтобы закрывать файлы и соединение после копирования
                        bytesReceived += bytesReadThisTime;
                    }
                }
            }
            // Закрываем соединение с сервером
            client.Close();
            Console.WriteLine("Копирование завершено.");

            // Закрываем консольное приложение
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();

        }

        public static void ExtractArchive(TcpClient client, NetworkStream stream)
        {

            // send the "ExtractArchive" request to the server

            string archiveRequest = "ExtractArchive";
           byte[] buffer = Encoding.UTF8.GetBytes(archiveRequest);
            stream.Write(buffer, 0, buffer.Length);
            // receive the list of archives from the server
            buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string archiveList = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            string[] archiveNames = archiveList.Split('\n');

            // display the list of archives to the user and ask for selection
            Console.WriteLine("Список архивов на сервере:");
            for (int i = 0; i < archiveNames.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {archiveNames[i]}");
            }
            Console.Write("Введите номер архива для распаковки: ");
            int selectedArchiveIndex = int.Parse(Console.ReadLine()) - 1;
            string selectedArchiveName = archiveNames[selectedArchiveIndex];

            // send the selected archive name to the server
            buffer = Encoding.UTF8.GetBytes(selectedArchiveName);
            stream.Write(buffer, 0, buffer.Length);

            // ask for the extraction path option and send it to the server
            Console.WriteLine("Выберите путь распаковки:");
            Console.WriteLine("1. Текущая директория сервера");
            Console.WriteLine("2. Задать свой путь");
            Console.Write("Введите номер варианта: ");
            string extractionOption = Console.ReadLine();
            buffer = Encoding.UTF8.GetBytes(extractionOption);
            stream.Write(buffer, 0, buffer.Length);

            if (extractionOption == "2")
            {
                // ask for the custom extraction path and send it to the server
                Console.Write("Введите путь для распаковки: ");
                string extractionPath = Console.ReadLine();
                buffer = Encoding.UTF8.GetBytes(extractionPath);
                stream.Write(buffer, 0, buffer.Length);
            }

            // ask for the password (if any) and send it to the server
            Console.Write("Введите пароль для распаковки (оставьте пустым, если нет пароля): ");
            string password = Console.ReadLine();
            buffer = Encoding.UTF8.GetBytes(password);
            stream.Write(buffer, 0, buffer.Length);

            // receive the result of the extraction from the server and display it to the user
            buffer = new byte[1024];
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            string resultMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            Console.WriteLine(resultMessage);

        }

    }


}

