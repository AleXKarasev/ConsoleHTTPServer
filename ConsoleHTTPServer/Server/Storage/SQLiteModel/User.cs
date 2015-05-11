using System;
using SQLite;

namespace Server.Storage.SqliteModel
{
    public class User
    {
        /// <summary>
        ///     Ключ пользователя
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        public String UserName { get; set; }
    }
}