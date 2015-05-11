using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Storage.SqliteModel;
using SQLite;

namespace Server.Storage
{
    public class StorageSQLight : IStorage
    {
        /// <summary>
        ///     Путь к БД
        /// </summary>
        private readonly string _dataBasePath;

        public StorageSQLight(String dataBasePath)
        {
            _dataBasePath = dataBasePath;

            if (!File.Exists(_dataBasePath))
            {
                // если БД не существовало, создаем структуру
                using (var db = new SQLiteConnection(_dataBasePath))
                {
                    db.CreateTable<User>();
                    db.CreateTable<Message>();
                }
            }
        }

        public void AddMessage(string user, string message)
        {
            // todo можно добавить транзакцию если предпологать что будет многопоточное обращение
            using (var db = new SQLiteConnection(_dataBasePath))
            {
                var curremtUser = db.Table<User>().FirstOrDefault(u => u.UserName == user);
                if (curremtUser == null)
                {
                    db.Insert(new User {UserName = user});
                    curremtUser = db.Table<User>().FirstOrDefault(u => u.UserName == user);
                }

                db.Insert(new Message {UserId = curremtUser.UserId, MessageBody = message});
            }
        }

        public IDictionary<string, string> GetAll()
        {
            var result = new Dictionary<string, string>();
            using (var db = new SQLiteConnection(_dataBasePath))
            {
                foreach (var userMessage in db.Query<UserMessage>(@"select User.UserName, Message.MessageBody 
                                                                    from User join Message on User.UserId == Message.UserId"))
                {
                    result.Add(userMessage.UserName, userMessage.MessageBody);
                }
            }
            return result;
        }
    }
}