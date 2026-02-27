namespace WpflLab1
{
    /// <summary>
    /// Модель привычки.
    /// Хранит название, время выполнения и статус завершения.
    /// </summary>
    public class Habit
    {
        /// <summary>
        /// Название привычки.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Время выполнения привычки.
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Признак выполнения привычки.
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
