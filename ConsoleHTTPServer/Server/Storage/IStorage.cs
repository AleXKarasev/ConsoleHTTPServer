using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace Server.Storage
{
    /// <summary>
    ///     Хранилище сообщений гостевой книги
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        ///     Добавляем сообщение
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        void AddMessage(String user, String message);

        /// <summary>
        ///     Возвращает все сообщения в виде словаря
        /// </summary>
        /// <returns></returns>
        IDictionary<String, String> GetAll();
    }
}