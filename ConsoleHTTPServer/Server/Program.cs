using System;
using System.Configuration;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = GetPort();
            using (var server = new HttpServer(port))
            {
                server.Start();
                Console.WriteLine("Слушаем порт {0}. Для завершения работы нажмите Enter...", port.ToString());
                Console.ReadLine();
            }
        }

        private static int GetPort()
        {
            string portConfig = ConfigurationManager.AppSettings["port"];
            int port;
            if (!Int32.TryParse(portConfig, out port))
            {
                port = 3000;
            }
            return port;
        }
    }
}
