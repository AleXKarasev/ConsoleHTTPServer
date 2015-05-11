using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Storage;

namespace Server.Test
{
    [TestClass]
    public class StorageSQLightTest
    {
        [TestMethod]
        public void StorageSQLightAddAndReadDataBase()
        {

            var tempPath = Path.GetTempFileName();

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            try
            {
                var target = new StorageSQLight(tempPath);
                target.AddMessage("test", "temp");
                target.AddMessage("AleXK", "Message");
                var result = target.GetAll();
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("temp", result["test"]);
                Assert.AreEqual("Message", result["AleXK"]);
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
