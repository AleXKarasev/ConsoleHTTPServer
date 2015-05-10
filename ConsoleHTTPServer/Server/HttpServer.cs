using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class HttpServer : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public HttpServer(Int32 port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _cancellationTokenSource = new CancellationTokenSource();
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

            ReturnHTMLHelloWorld(newClient);

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