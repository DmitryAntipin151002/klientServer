using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SevenZip;

namespace My_server
{
    public class Methods
    {
        public static void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // создаем новый объект StreamReader
                StreamReader streamReader = new StreamReader(stream);
                StreamWriter streamWriter = new StreamWriter(client.GetStream(), Encoding.UTF8);
                if (request.StartsWith("Copy computer to server"))
                {
                    try
                    {
                        // Получаем путь к папке с файлами на компьютере клиента
                        string folderPath = request.Substring(25);

                        // Запрашиваем у клиента список имен файлов для копирования
                        byte[] fileNamesBytes = new byte[1024];
                        int fileNamesBytesLength = stream.Read(fileNamesBytes, 0, fileNamesBytes.Length);
                        string[] fileNames = Encoding.UTF8.GetString(fileNamesBytes, 0, fileNamesBytesLength).Split(',');

                        foreach (string fileName in fileNames)
                        {
                            // Получаем путь к файлу на компьютере клиента
                            string filePath = Path.Combine(folderPath, fileName.Trim());

                            // Отправляем размер файла на сервер
                            long fileSize = new FileInfo(filePath).Length;
                            byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
                            stream.Write(fileSizeBytes, 0, fileSizeBytes.Length);

                            // Отправляем файл на сервер
                            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                            {
                                buffer = new byte[1024];
                                bytesRead = 0;
                                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    stream.Write(buffer, 0, bytesRead);
                                }
                            }

                            Console.WriteLine($"Файл {fileName} успешно скопирован на сервер.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке запроса клиента: {ex.Message}");
                    }
                }
                else if (request.StartsWith("list"))
                {
                    // обработка запроса на получение списка файлов в заданной директории
                    string directoryPath = request.Split(' ')[1];
                    string[] files = Directory.GetFiles(directoryPath);
                    string[] fileNames = files.Select(file => Path.GetFileName(file)).ToArray();
                    string fileList = string.Join("\n", fileNames);
                    buffer = Encoding.UTF8.GetBytes(fileList);
                    stream.Write(buffer, 0, buffer.Length);
                }
                else if (request.StartsWith("archive"))
                {
                    // receive the directory path from the client
                    buffer = new byte[1024];
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string directoryPath = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    string basePath = directoryPath;
                    string relativePath = Path.GetRelativePath(basePath, directoryPath);
                    string[] files = Directory.GetFiles(directoryPath);
                    string fileList = string.Join("\n", files);
                    string[] fileNames = fileList.Split('\n');
                    // send the list of files to the client
                    string fileListRequest = string.Join("\n", fileNames);
                    buffer = Encoding.UTF8.GetBytes(fileListRequest);
                    stream.Write(buffer, 0, buffer.Length);

                    // receive the selected file names, archive name, password, and compression format from the client
                    buffer = new byte[1024];
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string[] archiveSelection = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim().Split(',');
                    string[] selectedFileNames = archiveSelection[0].Trim().Split(';');
                    string archiveName = archiveSelection[1].Trim();
                    string password = archiveSelection[2].Trim();
                    string compressionFormat = archiveSelection[3].Trim().ToLower();
                    string archiveFilePath = Path.Combine(directoryPath, $"{archiveName}.{compressionFormat}");

                    // form the full paths to the selected files based on the selected file names
                    List<string> selectedFilePaths = new List<string>();
                    foreach (string selectedFileName in selectedFileNames)
                    {
                        string selectedFilePath = Path.Combine(directoryPath, selectedFileName);
                        if (!File.Exists(selectedFilePath))
                        {
                            string errorMessage = $"Файл '{selectedFileName}' не существует.";
                            buffer = Encoding.UTF8.GetBytes(errorMessage);
                            stream.Write(buffer, 0, buffer.Length);
                            return;
                        }
                        selectedFilePaths.Add(selectedFilePath);
                    }

                    try
                    {
                        // compress the selected files with the specified archive name, password, and compression format

                        if (compressionFormat == "1")
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo();
                            startInfo.FileName = "C:\\Users\\Ray_Gek\\Desktop\\Kyrsach\\My_server\\bin\\Архиватор\\WinRAR\\WinRAR.exe";
                            startInfo.WorkingDirectory = directoryPath;
                            startInfo.Arguments = $"a -ep1 -p{password} {archiveName}.rar {string.Join(" ", selectedFilePaths)}";
                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            using (Process process = Process.Start(startInfo))
                            {
                                process.WaitForExit();
                            }
                        }
                        else if (compressionFormat == "2")
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo();
                            startInfo.FileName = "C:\\Users\\Ray_Gek\\Desktop\\Kyrsach\\My_server\\bin\\Архиватор\\WinRAR\\WinRAR.exe";
                            startInfo.WorkingDirectory = directoryPath;
                            startInfo.Arguments = $"a -ep2 -p{password} {archiveName}.zip {string.Join(" ", selectedFilePaths)}";
                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            using (Process process = Process.Start(startInfo))
                            {
                                process.WaitForExit();
                            }
                        }

                        // send the success message to the client
                        string successMessage = $"Выбранные файлы успешно заархивированы.";
                        buffer = Encoding.UTF8.GetBytes(successMessage);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    catch (Exception ex)
                    {
                        // send the error message to the client if an exception occurred during archiving
                        string errorMessage = $"Ошибка при архивации выбранных файлов: {ex.Message}";
                        buffer = Encoding.UTF8.GetBytes(errorMessage);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
                if (request.StartsWith("Copy server to computer"))
                {
                    // Извлекаем из запроса путь к папке на сервере
                    string serverFolderPath = request.Replace("Copy server to computer", "");

                    // Получаем список имен файлов в папке на сервере
                    string[] fileNames = Directory.GetFiles(serverFolderPath);

                    // Отправляем список имен файлов клиенту
                    string fileNamesString = string.Join(",", fileNames);
                    byte[] fileNamesBytes = Encoding.UTF8.GetBytes(fileNamesString);
                    stream.Write(fileNamesBytes, 0, fileNamesBytes.Length);

                    // Читаем выбранные файлы от клиента
                    byte[] selectedFilesBytes = new byte[1024];
                    int selectedFilesBytesLength = stream.Read(selectedFilesBytes, 0, selectedFilesBytes.Length);
                    string selectedFilesString = Encoding.UTF8.GetString(selectedFilesBytes, 0, selectedFilesBytesLength);

                    // Извлекаем из выбранных файлов список имен
                    string[] selectedFiles = selectedFilesString.Split(',');

                    // Отправляем выбранные файлы клиенту
                    foreach (string fileName in selectedFiles)
                    {
                        // Отправляем размер файла на клиент
                        string filePath = Path.Combine(serverFolderPath, fileName);
                        long fileSize = new FileInfo(filePath).Length;
                        byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
                        stream.Write(fileSizeBytes, 0, fileSizeBytes.Length);

                        // Отправляем содержимое файла клиенту
                        buffer = new byte[1024];
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                stream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }

                if (request.StartsWith("ExtractArchive"))
                {
                    // get the list of archives on the server
                    Config config = new Config();
                    string directoryPath = config.Path;
                    string[] archives = Directory.GetFiles(directoryPath);
                    string archiveList = string.Join("\n", archives);
                    string[] archiveNames = archiveList.Split('\n');

                    // send the list of archives to the client
                    string archiveListRequest = string.Join("\n", archiveNames);
                    buffer = Encoding.UTF8.GetBytes(archiveListRequest);
                    stream.Write(buffer, 0, buffer.Length);

                    // receive the selected archive name from the client
                    buffer = new byte[1024];
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string selectedArchiveName = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    string selectedArchivePath = Path.Combine(directoryPath, selectedArchiveName);

                    // check if the selected archive exists
                    if (!File.Exists(selectedArchivePath))
                    {
                        string errorMessage = $"Архив '{selectedArchiveName}' не существует.";
                        buffer = Encoding.UTF8.GetBytes(errorMessage);
                        stream.Write(buffer, 0, buffer.Length);
                        return;
                    }

                    // receive the extraction path option from the client
                    buffer = new byte[1024];
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string extractionOption = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    string extractionPath;
                    if (extractionOption == "1")
                    {
                        // extract the files to the server directory
                        extractionPath = directoryPath;
                    }
                    else if (extractionOption == "2")
                    {
                        // receive the custom extraction path from the client
                        buffer = new byte[1024];
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        extractionPath = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        // check if the custom extraction path exists
                        if (!Directory.Exists(extractionPath))
                        {
                            string errorMessage = $"Путь '{extractionPath}' не существует.";
                            buffer = Encoding.UTF8.GetBytes(errorMessage);
                            stream.Write(buffer, 0, buffer.Length);
                            return;
                        }
                    }
                    else
                    {
                        string errorMessage = $"Некорректный вариант выбора пути распаковки: {extractionOption}.";
                        buffer = Encoding.UTF8.GetBytes(errorMessage);
                        stream.Write(buffer, 0, buffer.Length);
                        return;
                    }

                    // receive the password (if any) from the client
                    buffer = new byte[1024];
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string password = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    try
                    {
                        // extract the files from the selected archive to the extraction path
                        if (!string.IsNullOrEmpty(password))
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo();
                            startInfo.FileName = "C:\\Users\\Ray_Gek\\Desktop\\Kyrsach\\My_server\\bin\\Архиватор\\WinRAR\\WinRAR.exe";
                            startInfo.WorkingDirectory = directoryPath;
                            startInfo.Arguments = $"x -p{password} {selectedArchiveName} {extractionPath}";
                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            using (Process process = Process.Start(startInfo))
                            {
                                process.WaitForExit();
                            }
                        }
                        else
                        {
                            ZipFile.ExtractToDirectory(selectedArchiveName, extractionPath);
                        }
                        // send a success message to the client
                        string successMessage = $"Архив '{selectedArchiveName}' успешно распакован в папку '{extractionPath}'.";
                        buffer = Encoding.UTF8.GetBytes(successMessage);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    catch (Exception ex)
                    {
                        // send an error message to the client
                        string errorMessage = $"Ошибка при распаковке архива '{selectedArchiveName}': {ex.Message}.";
                        buffer = Encoding.UTF8.GetBytes(errorMessage);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
                else if (request == "exit")
                {
                    // отправка подтверждения отключения клиента
                    buffer = Encoding.UTF8.GetBytes("bye");
                    stream.Write(buffer, 0, buffer.Length);

                    // разрыв соединения и выход из цикла обработки запросов
                    client.Close();
                }
                else
                {
                    // неверный запрос
                    Console.WriteLine("Клиент отправил неверный запрос.");
                }
            }
            catch
            {
                Console.WriteLine("Ошибка при обработке запроса клиента.");
            }
        }
    }
}
