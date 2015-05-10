using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Server.Storage
{
    public class StorageXml : IStorage
    {
        private readonly string _filePath;

        public StorageXml(String filePath)
        {
            _filePath = filePath;
        }

        public void AddMessage(string user, string message)
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists(_filePath))
            {
                doc.Load(_filePath);
            }
            else
            {
                var root = doc.CreateElement("mesages");
                doc.AppendChild(root);
            }

            XmlElement newMessage = doc.CreateElement(user);
            newMessage.InnerText = message;

            doc.SelectSingleNode("mesages").AppendChild(newMessage);

            doc.Save(_filePath);
        }

        public IDictionary<String, String> GetAll()
        {
            var result = new Dictionary<String, String>();
            XmlDocument doc = new XmlDocument();
            doc.Load(_filePath);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                result.Add(node.Name, node.InnerText);
            }
            return result;
        }
    }
}