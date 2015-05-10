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
            Task.Factory.StartNew(ServerMainThread);
        }

        private void ServerMainThread()
        {
            _listener.Start();
            AcceptClientsAsync(_listener, _cancellationTokenSource.Token);
        }

        async Task AcceptClientsAsync(TcpListener listener, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                EchoAsync(client, ct);
            }
        }

        async Task EchoAsync(TcpClient client, CancellationToken ct)
        {
            Console.WriteLine("New client connected");
            using (client)
            {
                var stream = client.GetStream();
                while (!ct.IsCancellationRequested)
                {
                    // сожержимое страницы
                    const string html = "<html><body><h1>Hello world!</h1></body></html>";
                    // + заголовок
                    string str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + html.Length.ToString() + "\n\n" + html;
                    // Приведем строку к виду массива байт
                    byte[] buffer = Encoding.ASCII.GetBytes(str);

                    //await stream.WriteAsync(buffer, 0, buffer.Length, ct);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
            Console.WriteLine("Client disconnected");
        }

        public void Dispose()
        {
            // останавливаем основной поток сервера
            _cancellationTokenSource.Cancel();
            _listener.Stop();
        }
    }
}