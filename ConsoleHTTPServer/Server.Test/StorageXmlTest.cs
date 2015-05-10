using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Storage;

namespace Server.Test
{
    [TestClass]
    public class StorageXmlTest
    {
        [TestMethod]
        public void StorageXmlWriteAndReadTest()
        {
            var tempPath = Path.GetTempFileName();

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            try
            {
                var target = new StorageXml(tempPath);

                target.AddMessage("test", "temp");
                var result = target.GetAll();

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("temp", result["test"]);
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }
    }
}
