using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server
{
    /// <summary>
    ///     Парсер HTTP заголовков
    /// </summary>
    public class ParserHttpHeader
    {
        /// <summary>
        ///     Типа запроса get или post
        /// </summary>
        private RequestType _requestType;
        /// <summary>
        ///     Маршрут сообщения
        /// </summary>
        private String _route;
        /// <summary>
        ///     Загловки сообщения
        /// </summary>
        private readonly Dictionary<String, String> _headers = new Dictionary<String, String>();
        /// <summary>
        ///     Данные сообщения
        /// </summary>
        private Dictionary<String, String> _data;

        public ParserHttpHeader(String request)
        {
            Boolean isBody = false;
            foreach (var line in request.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (line.Length == 0)
                {
                    // если пришла пустая стрка, занчит заголовок закончился и началось тело
                    isBody = true;
                    continue;
                }

                if (String.IsNullOrEmpty(_route))
                {
                    // значит мы не прочитали еще первую строку
                    ParseFirstLine(line);
                }
                else if (!isBody)
                {
                    var poz = line.IndexOf(":", StringComparison.Ordinal);
                    _headers.Add(line.Substring(0, poz).Trim(), line.Substring(poz + 1, line.Length - poz - 1).Trim());
                }
                else
                {
                    // дошли до данных post запроса
                    ParseQueryString(line);
                }
            }
        }

        /// <summary>
        ///     Парсит строку параметров
        /// </summary>
        /// <param name="data"></param>
        private void ParseQueryString(String data)
        {
            var nvc = HttpUtility.ParseQueryString(data);
            _data = nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }

        /// <summary>
        ///     Типа запроса get или post
        /// </summary>
        public RequestType RequestType
        {
            get { return _requestType; }
        }

        /// <summary>
        ///     Маршрут сообщения
        /// </summary>
        public string Route
        {
            get { return _route; }
        }

        /// <summary>
        ///     Загловки сообщения
        /// </summary>
        public Dictionary<string, string> Headers
        {
            get { return _headers; }
        }

        /// <summary>
        ///     Данные сообщения
        /// </summary>
        public Dictionary<string, string> Data
        {
            get { return _data; }
        }

        /// <summary>
        ///     Парсер первой строки
        /// </summary>
        /// <param name="firstLine"></param>
        private void ParseFirstLine(String firstLine)
        {
            var array = firstLine.Split(' ');
            _requestType = array[0].ToUpper().Trim().Equals("GET") ? RequestType.Get : RequestType.Post;
            if (_requestType == RequestType.Get && array[1].Contains("?"))
            {
                var poz = array[1].IndexOf("?", StringComparison.Ordinal);
                _route = array[1].Substring(0, poz).Trim();
                ParseQueryString(array[1].Substring(poz + 1, array[1].Length - poz - 1).Trim());
            }
            else
            {
                _route = array[1];
            }
        }
    }

    public enum RequestType
    {
        Get,
        Post
    }
}