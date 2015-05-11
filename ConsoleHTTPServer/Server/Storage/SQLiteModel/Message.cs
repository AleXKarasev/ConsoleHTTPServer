using SQLite;

namespace Server.Storage.SqliteModel
{
    public class Message
    {
        /// <summary>
        ///     Ключ сообщения
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int MessageId { get; set; }

        /// <summary>
        ///     Ключ пользователя которому принадлежит сообщение
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Тело сообщения
        /// </summary>
        public string MessageBody { get; set; }
    }
}