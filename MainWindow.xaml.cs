using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpflLab1
{
    /// <summary>
    /// Главное окно приложения "Управление привычками".
    /// Содержит трёхпанельный интерфейс с меню, панелью инструментов и строкой состояния.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Текущий профиль пользователя.
        /// </summary>
        private UserProfile _currentProfile;

        /// <summary>
        /// Коллекция привычек для отображения в DataGrid.
        /// </summary>
        private ObservableCollection<Habit> _habits;

        /// <summary>
        /// Коллекция дневной статистики для отображения в ListView.
        /// </summary>
        private ObservableCollection<DailyStatistic> _statistics;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MainWindow"/>.
        /// Загружает компоненты пользовательского интерфейса и настраивает начальное состояние.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _habits = new ObservableCollection<Habit>();
            HabitsDataGrid.ItemsSource = _habits;
            HabitsCalendar.SelectedDate = DateTime.Today;

            _statistics = new ObservableCollection<DailyStatistic>
            {
                new DailyStatistic { Date = "25.02.2026", CompletedCount = 5, Percentage = "83%" },
                new DailyStatistic { Date = "26.02.2026", CompletedCount = 3, Percentage = "50%" },
                new DailyStatistic { Date = "27.02.2026", CompletedCount = 6, Percentage = "100%" },
                new DailyStatistic { Date = "28.02.2026", CompletedCount = 4, Percentage = "67%" },
            };
            StatsListView.ItemsSource = _statistics;
        }

        /// <summary>
        /// Проверяет корректность введённых данных на вкладке "Личные данные".
        /// </summary>
        /// <returns>Строка с описанием ошибки или null, если данные корректны.</returns>
        private string ValidateProfile()
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
                return "Введите имя.";

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
                return "Введите фамилию.";

            if (BirthDatePicker.SelectedDate.HasValue && BirthDatePicker.SelectedDate.Value > DateTime.Today)
                return "Дата рождения не может быть в будущем.";

            return null;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить данные".
        /// Валидирует ввод и сохраняет данные в объект <see cref="UserProfile"/>.
        /// </summary>
        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            string error = ValidateProfile();
            if (error != null)
            {
                MessageBox.Show(error, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentProfile = new UserProfile
            {
                FirstName = FirstNameTextBox.Text.Trim(),
                LastName = LastNameTextBox.Text.Trim(),
                Password = PasswordBox.Password,
                BirthDate = BirthDatePicker.SelectedDate,
                Education = (EducationComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                Hobbies = GetSelectedHobbies(),
                NotificationsEnabled = NotificationsCheckBox.IsChecked == true,
                PublicStatistics = PublicStatsCheckBox.IsChecked == true,
                AutoSave = AutoSaveCheckBox.IsChecked == true,
                ActivityLevel = GetSelectedActivityLevel()
            };

            MessageBox.Show("Данные успешно сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Собирает список выбранных хобби из ListBox.
        /// </summary>
        /// <returns>Список строк с названиями выбранных хобби.</returns>
        private List<string> GetSelectedHobbies()
        {
            var hobbies = new List<string>();
            foreach (ListBoxItem item in HobbiesListBox.SelectedItems)
            {
                hobbies.Add(item.Content.ToString());
            }
            return hobbies;
        }

        /// <summary>
        /// Определяет выбранный уровень активности по состоянию RadioButton.
        /// </summary>
        /// <returns>Строка с уровнем активности или null, если ни один не выбран.</returns>
        private string GetSelectedActivityLevel()
        {
            if (LowActivityRadio.IsChecked == true) return "Низкий";
            if (MediumActivityRadio.IsChecked == true) return "Средний";
            if (HighActivityRadio.IsChecked == true) return "Высокий";
            return null;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сброс".
        /// Очищает все поля на вкладке "Личные данные".
        /// </summary>
        private void ResetProfile_Click(object sender, RoutedEventArgs e)
        {
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            PasswordBox.Clear();
            BirthDatePicker.SelectedDate = null;
            EducationComboBox.SelectedIndex = -1;
            HobbiesListBox.UnselectAll();
            NotificationsCheckBox.IsChecked = false;
            PublicStatsCheckBox.IsChecked = false;
            AutoSaveCheckBox.IsChecked = false;
            LowActivityRadio.IsChecked = false;
            MediumActivityRadio.IsChecked = false;
            HighActivityRadio.IsChecked = false;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Добавить".
        /// Создаёт новую привычку и добавляет её в коллекцию.
        /// </summary>
        private void AddHabit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HabitNameTextBox.Text))
            {
                MessageBox.Show("Введите название привычки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _habits.Add(new Habit
            {
                Name = HabitNameTextBox.Text.Trim(),
                Time = HabitTimeTextBox.Text.Trim(),
                IsCompleted = false
            });

            HabitNameTextBox.Clear();
            HabitTimeTextBox.Clear();
        }

        /// <summary>
        /// Обработчик изменения значения слайдеров продуктивности и удовлетворённости.
        /// Обновляет текстовые подписи и пересчитывает прогресс дня как среднее двух значений.
        /// </summary>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ProductivityText == null || SatisfactionText == null || DayProgressBar == null)
                return;

            ProductivityText.Text = $"{(int)ProductivitySlider.Value}%";
            SatisfactionText.Text = $"{(int)SatisfactionSlider.Value}%";
            DayProgressBar.Value = (ProductivitySlider.Value + SatisfactionSlider.Value) / 2;
        }

        /// <summary>
        /// Обработчик переключения вкладок.
        /// Обновляет строку состояния в зависимости от выбранной вкладки.
        /// </summary>
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusText == null || HabitCountText == null)
                return;

            switch (MainTabControl.SelectedIndex)
            {
                case 0:
                    StatusText.Text = "Редактирование профиля";
                    break;
                case 1:
                    StatusText.Text = "Управление привычками";
                    break;
                case 2:
                    StatusText.Text = "Просмотр статистики";
                    break;
            }

            HabitCountText.Text = $"Привычек: {_habits?.Count ?? 0}";
        }

        /// <summary>
        /// Обработчик выбора элемента в TreeView категорий.
        /// Обновляет строку состояния названием выбранной категории.
        /// </summary>
        private void StatsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (StatusText == null)
                return;

            var selectedItem = e.NewValue as TreeViewItem;
            if (selectedItem != null)
            {
                StatusText.Text = $"Выбрана категория: {selectedItem.Header}";
            }
        }
    }
}
