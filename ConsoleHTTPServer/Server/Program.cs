using System;
using System.Configuration;
using Ninject;
using Server.Storage;

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


        private static IKernel _kernel;

        /// <summary>
        ///     Возвращает ядро Ninject.
        /// </summary>
        /// <returns>Созданное ядро Ninject.</returns>
        public static IKernel GetKernel()
        {
            _kernel = new StandardKernel();
            try
            {
                RegisterServices(_kernel);
                return _kernel;
            }
            catch
            {
                _kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        ///     Загрузка модулей Ninject и регистрация привязок.
        ///     Загрузка модулей Ninject и регистрация привязок.
        /// </summary>
        /// <param name="kernel">Ядро Ninject.</param>
        private static void RegisterServices(IKernel kernel)
        {
            string dataFilePath = ConfigurationManager.AppSettings["dataFilePath"] ?? "c:\\temp.xml";
            Boolean isXml = ConfigurationManager.AppSettings["storeType"] == "xml";
            // в зависимости от конфига биндим нужный сторадж
            if (isXml)
            {
                kernel.Bind<IStorage>()
                    .ToConstant(new StorageXml(dataFilePath))
                    .InSingletonScope();
            }
            else
            {
                kernel.Bind<IStorage>()
                    .ToConstant(new StorageSQLight(dataFilePath))
                    .InSingletonScope();
            }
        }
    }
}
