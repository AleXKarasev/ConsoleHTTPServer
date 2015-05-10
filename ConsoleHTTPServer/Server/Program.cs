using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // todo читаем порт из конфига
            var port = 3000;

            using (var server = new HttpServer(port))
            {
                server.Start();
                Console.WriteLine("Слушаем порт {0}. Для завершения работы нажмите Enter...", port.ToString());
                Console.ReadLine();
            }
        }
    }
}
