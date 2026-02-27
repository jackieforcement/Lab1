namespace WpflLab1
{
    /// <summary>
    /// Модель дневной статистики.
    /// Хранит дату, количество выполненных привычек и процент выполнения.
    /// </summary>
    public class DailyStatistic
    {
        /// <summary>
        /// Дата записи статистики.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Количество выполненных привычек за день.
        /// </summary>
        public int CompletedCount { get; set; }

        /// <summary>
        /// Процент выполнения привычек за день.
        /// </summary>
        public string Percentage { get; set; }
    }
}
