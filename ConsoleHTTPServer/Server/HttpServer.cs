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
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            Task.Factory.StartNew(ServerMainThread);
        }

        private void ServerMainThread()
        {
            _listener.Start();

            while (true)
            {
                var newClient = _listener.AcceptTcpClient();
                Task.Factory.StartNew(() => RequestProcess(newClient));
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
            String finishChar = "\r\n\r\n";
            // читаем по килобайту
            byte[] Buffer = new byte[1024];
            int Count;
            // Читаем из потока клиента до тех пор, пока от него поступают данные
            while ((Count = newClient.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {
                // Преобразуем эти данные в строку и добавим ее к переменной Request
                result += Encoding.ASCII.GetString(Buffer, 0, Count);
                if (result.IndexOf(finishChar, StringComparison.Ordinal) >= 0)
                {
                    // обрезаем строку и выходим
                    result = result.Substring(0, result.IndexOf(finishChar, StringComparison.Ordinal));
                    break;
                }
            }

            return result;
        }

        public void Dispose()
        {
            // останавливаем основной поток сервера
            _listener.Stop();
        }
    }
}