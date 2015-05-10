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

        public HttpServer(Int32 port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                _listener.Start();
                ServerMainThread();
            });
        }

        private void ServerMainThread()
        {
            _listener.BeginAcceptTcpClient(RequestProcess, _listener);
        }

        private void RequestProcess(IAsyncResult res)
        {
            ServerMainThread();
            TcpClient client = _listener.EndAcceptTcpClient(res);

            // Код простой HTML-странички
            string Html = "<html><body><h1>Hello world!</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            client.Close();
        }

        public void Dispose()
        {
            // останавливаем основной поток сервера
            //_canceller.Cancel();
            _listener.Stop();
        }
    }
}