using System;
using System.Collections.Generic;

namespace WpflLab1
{
    /// <summary>
    /// Модель данных пользователя.
    /// Хранит персональные данные, настройки отображения и уровень активности.
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия пользователя.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Дата рождения пользователя.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Уровень образования пользователя.
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// Список выбранных хобби.
        /// </summary>
        public List<string> Hobbies { get; set; }

        /// <summary>
        /// Флаг получения уведомлений.
        /// </summary>
        public bool NotificationsEnabled { get; set; }

        /// <summary>
        /// Флаг публичного отображения статистики.
        /// </summary>
        public bool PublicStatistics { get; set; }

        /// <summary>
        /// Флаг автоматического сохранения данных.
        /// </summary>
        public bool AutoSave { get; set; }

        /// <summary>
        /// Уровень активности пользователя (Низкий, Средний, Высокий).
        /// </summary>
        public string ActivityLevel { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UserProfile"/> со значениями по умолчанию.
        /// </summary>
        public UserProfile()
        {
            Hobbies = new List<string>();
        }
    }
}
