﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Storage;

namespace Server
{
    public class HttpServer : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IStorage _storage;

        public HttpServer(Int32 port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _cancellationTokenSource = new CancellationTokenSource();
            _storage = new StorageXml("c:\\temp.xml");
        }

        public void Start()
        {
            Task.Factory.StartNew(ServerMainThread, _cancellationTokenSource.Token);
        }

        private void ServerMainThread()
        {
            _listener.Start();

            while (true)
            {
                var newClient = _listener.AcceptTcpClient();
                Task.Factory.StartNew(() => RequestProcess(newClient));

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }

        private void RequestProcess(TcpClient newClient)
        {
            var clientRequest = ReadClientRequest(newClient);

            Console.WriteLine("%%%%%%%%%%%%%%%% Новый запрос %%%%%%%%%%%%%%%%");
            Console.WriteLine(clientRequest);
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");

            var parser = new ParserHttpHeader(clientRequest);
            
            if (parser.Route == "/Guestbook/")
            {
                if (parser.RequestType == RequestType.Get)
                {
                    // возвращаем все записи из гостевой книги
                    ReturnAllMessage(newClient, _storage.GetAll());
                }
                else if (parser.RequestType == RequestType.Post)
                {
                    // если есть user и message то пишем их или в БД или в XML
                    if (parser.Data.ContainsKey("user") && parser.Data.ContainsKey("message"))
                    {
                        //  todo нужно обработать соосбщения на наличие sql injection
                        _storage.AddMessage(parser.Data["user"], parser.Data["message"]);
                    }
                }
            }
            else if (parser.Route == "/Proxy/")
            {
                // todo нужно загрузить данные из переданного url и вернуть их в ответе
            }
            else
            {
                // стандартное приветствие
                ReturnHTMLHelloWorld(newClient);
            }

            newClient.Close();
        }

        private void ReturnHTMLHelloWorld(TcpClient newClient)
        {
            // сожержимое страницы
            const string html = "<html><body><h1>Hello world!</h1></body></html>";
            // + заголовок
            string str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + html.Length.ToString() + "\n\n" + html;
            // Приведем строку к виду массива байт
            byte[] buffer = Encoding.ASCII.GetBytes(str);
            
            newClient.GetStream().Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        ///     Возвращает все сообщения в гостевой книге
        /// </summary>
        /// <param name="newClient"></param>
        /// <param name="allMessage"></param>
        private void ReturnAllMessage(TcpClient newClient, IDictionary<String, String> allMessage)
        {
            // сожержимое страницы
            var htmp = new StringBuilder();
            htmp.AppendLine("<html>");
            htmp.AppendLine("   <body>");

            foreach (var message in allMessage)
            {
                htmp.AppendLine(String.Format("   <h3>{0}: {1}</h3>", message.Key, message.Value));
            }

            htmp.AppendLine("   </body>");
            htmp.AppendLine("</html>");
            // + заголовок
            string str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + htmp.Length.ToString() + "\n\n" + htmp.ToString();
            // Приведем строку к виду массива байт
            byte[] buffer = Encoding.ASCII.GetBytes(str);

            newClient.GetStream().Write(buffer, 0, buffer.Length);
        }

        private String ReadClientRequest(TcpClient newClient)
        {
            String result = String.Empty;

            var myNetworkStream = newClient.GetStream();
            var buffer = new byte[1024];
            do
            {
                int bytesRead = myNetworkStream.Read(buffer, 0, buffer.Length);
                result += Encoding.ASCII.GetString(buffer, 0, bytesRead);
            }
            while (myNetworkStream.DataAvailable);
            
            return result;
        }

        public void Dispose()
        {
            // останавливаем основной поток сервера
            _cancellationTokenSource.Cancel();
        }
    }
}