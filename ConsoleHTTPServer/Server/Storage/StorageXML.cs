using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Server.Storage
{
    public class StorageXml : IStorage
    {
        private readonly string _filePath;

        public StorageXml(String filePath)
        {
            _filePath = filePath;

            if (!File.Exists(_filePath))
            {
                XmlDocument doc = new XmlDocument();
                var root = doc.CreateElement("mesages");
                doc.AppendChild(root);
                doc.Save(_filePath);
            }
        }

        public void AddMessage(string user, string message)
        {
            // todo можно добавить блокировку на создаение/дозаписть в файл если предпологать что будет многопоточное обращение
            XmlDocument doc = new XmlDocument();
            doc.Load(_filePath);

            XmlElement newMessage = doc.CreateElement(user);
            newMessage.InnerText = message;

            doc.SelectSingleNode("mesages").AppendChild(newMessage);

            doc.Save(_filePath);
        }

        public IDictionary<String, String> GetAll()
        {
            var result = new Dictionary<String, String>();
            if (File.Exists(_filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(_filePath);
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    result.Add(node.Name, node.InnerText);
                }
            }
            return result;
        }
    }
}