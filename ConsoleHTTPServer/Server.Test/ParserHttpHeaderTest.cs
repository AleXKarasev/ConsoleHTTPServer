using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Server.Test
{
    [TestClass]
    public class ParserHttpHeaderTest
    {
        [TestMethod]
        public void ParserHttpHeaderGetRequest()
        {
            StringBuilder request = new StringBuilder();
            request.AppendLine("GET /ACTION=TEST HTTP/1.1");
            request.AppendLine("Host: localhost:8080");
            request.AppendLine("User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; en-GB; rv:1.9.0.10) Gecko/2009042316 Firefox/3.0.10");
            request.AppendLine("Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.AppendLine("Accept-Language: en-gb,en;q=0.5");
            request.AppendLine("Accept-Encoding: gzip,deflate");
            request.AppendLine("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            request.AppendLine("Keep-Alive: 300");
            request.AppendLine("Connection: keep-alive");
            request.AppendLine("");

            var target = new ParserHttpHeader(request.ToString());
            Assert.AreEqual(RequestType.Get, target.RequestType);
            Assert.AreEqual("/ACTION=TEST", target.Route);
        }

        [TestMethod]
        public void ParserHttpHeaderPostRequest()
        {
            StringBuilder request = new StringBuilder();
            request.AppendLine("POST /Guestbook/ HTTP/1.1");
            request.AppendLine("Host: localhost:3000");
            request.AppendLine("Connection: keep-alive");
            request.AppendLine("Content-Length: 34");
            request.AppendLine("Cache-Control: no-cache");
            request.AppendLine("Origin: chrome-extension://fdmmgilgnpjigdojojpjoooidkmcomcm");
            request.AppendLine("User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36");
            request.AppendLine("Content-Type: application/x-www-form-urlencoded");
            request.AppendLine("Accept: */*");
            request.AppendLine("Accept-Encoding: gzip, deflate");
            request.AppendLine("Accept-Language: ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            request.AppendLine("");
            request.AppendLine("user=test&message=dfg+dfg+dfg+fdh+");

            var target = new ParserHttpHeader(request.ToString());
            Assert.AreEqual(RequestType.Post, target.RequestType);
            Assert.AreEqual("/Guestbook/", target.Route);
            Assert.AreEqual("test", target.Data["user"]);
            Assert.AreEqual("dfg dfg dfg fdh ", target.Data["message"]);
        }
    }
}
