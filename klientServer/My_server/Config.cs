using System.IO;

namespace My_server
{
    public class Config
    {
        public string Path { get; }
        public string Ip { get; }
        public int Port { get; }

        public Config()
        {
            string[] lines = File.ReadAllLines("config.ini");
            foreach (string line in lines)
            {
                string[] parts = line.Split('=');
                string key = parts[0].Trim().ToLower();
                string value = parts[1].Trim();
                switch (key)
                {
                    case "path":
                        Path = value;
                        break;
                    case "ip":
                        Ip = value;
                        break;
                    case "port":
                        int.TryParse(value, out int port);
                        Port = port;
                        break;
                    default:
                        // неизвестный ключ
                        break;
                }
            }
        }
    }
}
