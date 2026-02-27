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
using System.Windows.Controls.Primitives;
using Microsoft.Win32;

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

        /// <summary>
        /// Применяет тёмную или светлую тему ко всему интерфейсу,
        /// переопределяя системные цвета и добавляя стили для всех типов контролов.
        /// </summary>
        /// <param name="isDark">true — тёмная тема, false — светлая.</param>
        private void ApplyTheme(bool isDark)
        {
            if (isDark)
            {
                var darkBg = new SolidColorBrush(Color.FromRgb(45, 45, 48));
                var controlBg = new SolidColorBrush(Color.FromRgb(62, 62, 66));
                var inputBg = new SolidColorBrush(Color.FromRgb(51, 51, 55));
                var lightText = new SolidColorBrush(Colors.WhiteSmoke);
                var border = new SolidColorBrush(Color.FromRgb(100, 100, 100));

                Background = darkBg;
                Foreground = lightText;

                Resources[SystemColors.WindowBrushKey] = darkBg;
                Resources[SystemColors.WindowTextBrushKey] = lightText;
                Resources[SystemColors.ControlBrushKey] = controlBg;
                Resources[SystemColors.ControlTextBrushKey] = lightText;
                Resources[SystemColors.ActiveBorderBrushKey] = border;
                Resources[SystemColors.InactiveBorderBrushKey] = border;

                foreach (var t in new[] { typeof(Button), typeof(ToggleButton), typeof(RepeatButton) })
                    Resources[t] = MakeStyle(t,
                        new Setter(Control.BackgroundProperty, controlBg),
                        new Setter(Control.ForegroundProperty, lightText),
                        new Setter(Control.BorderBrushProperty, border));

                Resources[typeof(TabControl)] = MakeStyle(typeof(TabControl),
                    new Setter(Control.BackgroundProperty, darkBg),
                    new Setter(Control.BorderBrushProperty, border));
                Resources[typeof(TabItem)] = MakeStyle(typeof(TabItem),
                    new Setter(Control.BackgroundProperty, controlBg),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(Control.BorderBrushProperty, border));

                Resources[typeof(TextBox)] = MakeStyle(typeof(TextBox),
                    new Setter(Control.BackgroundProperty, inputBg),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(TextBox.CaretBrushProperty, lightText),
                    new Setter(Control.BorderBrushProperty, border));
                Resources[typeof(RichTextBox)] = MakeStyle(typeof(RichTextBox),
                    new Setter(Control.BackgroundProperty, inputBg),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(Control.BorderBrushProperty, border));
                Resources[typeof(PasswordBox)] = MakeStyle(typeof(PasswordBox),
                    new Setter(Control.BackgroundProperty, inputBg),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(Control.BorderBrushProperty, border));
                Resources[typeof(ComboBox)] = MakeStyle(typeof(ComboBox),
                    new Setter(Control.BackgroundProperty, controlBg),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(Control.BorderBrushProperty, border));
                Resources[typeof(DatePicker)] = MakeStyle(typeof(DatePicker),
                    new Setter(Control.BackgroundProperty, inputBg),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(Control.BorderBrushProperty, border));

                foreach (var t in new[] { typeof(ListBox), typeof(ListView), typeof(TreeView) })
                    Resources[t] = MakeStyle(t,
                        new Setter(Control.BackgroundProperty, inputBg),
                        new Setter(Control.ForegroundProperty, lightText),
                        new Setter(Control.BorderBrushProperty, border));

                Resources[typeof(DataGrid)] = MakeStyle(typeof(DataGrid),
                    new Setter(Control.BackgroundProperty, darkBg),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(DataGrid.RowBackgroundProperty, darkBg),
                    new Setter(DataGrid.AlternatingRowBackgroundProperty, controlBg),
                    new Setter(DataGrid.GridLinesVisibilityProperty, DataGridGridLinesVisibility.None),
                    new Setter(Control.BorderBrushProperty, border));
                Resources[typeof(DataGridColumnHeader)] = MakeStyle(typeof(DataGridColumnHeader),
                    new Setter(Control.BackgroundProperty, controlBg),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(Control.BorderBrushProperty, border));
                Resources[typeof(GridViewColumnHeader)] = MakeStyle(typeof(GridViewColumnHeader),
                    new Setter(Control.BackgroundProperty, controlBg),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(Control.BorderBrushProperty, border));

                Resources[typeof(GroupBox)] = MakeStyle(typeof(GroupBox),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(Control.BorderBrushProperty, border));
                Resources[typeof(Label)] = MakeStyle(typeof(Label),
                    new Setter(Control.ForegroundProperty, lightText));
                Resources[typeof(CheckBox)] = MakeStyle(typeof(CheckBox),
                    new Setter(Control.ForegroundProperty, lightText));
                Resources[typeof(RadioButton)] = MakeStyle(typeof(RadioButton),
                    new Setter(Control.ForegroundProperty, lightText));

                Resources[typeof(Menu)] = MakeStyle(typeof(Menu),
                    new Setter(Control.BackgroundProperty, controlBg),
                    new Setter(Control.ForegroundProperty, lightText));
                Resources[typeof(MenuItem)] = MakeStyle(typeof(MenuItem),
                    new Setter(Control.ForegroundProperty, lightText));
                Resources[typeof(ToolBarTray)] = MakeStyle(typeof(ToolBarTray),
                    new Setter(Control.BackgroundProperty, controlBg));
                Resources[typeof(ToolBar)] = MakeStyle(typeof(ToolBar),
                    new Setter(Control.BackgroundProperty, controlBg),
                    new Setter(Control.ForegroundProperty, lightText));
                Resources[typeof(StatusBar)] = MakeStyle(typeof(StatusBar),
                    new Setter(Control.BackgroundProperty, controlBg),
                    new Setter(Control.ForegroundProperty, lightText));

                Resources[typeof(ScrollViewer)] = MakeStyle(typeof(ScrollViewer),
                    new Setter(Control.BackgroundProperty, darkBg));
                Resources[typeof(GridSplitter)] = MakeStyle(typeof(GridSplitter),
                    new Setter(Control.BackgroundProperty, border));
                Resources[typeof(Calendar)] = MakeStyle(typeof(Calendar),
                    new Setter(Control.BackgroundProperty, controlBg),
                    new Setter(Control.ForegroundProperty, lightText),
                    new Setter(Control.BorderBrushProperty, border));
                Resources[typeof(ProgressBar)] = MakeStyle(typeof(ProgressBar),
                    new Setter(Control.BackgroundProperty, controlBg),
                    new Setter(Control.BorderBrushProperty, border));
            }
            else
            {
                Background = SystemColors.WindowBrush;
                Foreground = SystemColors.WindowTextBrush;

                var keys = new object[]
                {
                    SystemColors.WindowBrushKey, SystemColors.WindowTextBrushKey,
                    SystemColors.ControlBrushKey, SystemColors.ControlTextBrushKey,
                    SystemColors.ActiveBorderBrushKey, SystemColors.InactiveBorderBrushKey,
                    typeof(Button), typeof(ToggleButton), typeof(RepeatButton),
                    typeof(TabControl), typeof(TabItem),
                    typeof(TextBox), typeof(RichTextBox), typeof(PasswordBox),
                    typeof(ComboBox), typeof(DatePicker),
                    typeof(ListBox), typeof(ListView), typeof(TreeView),
                    typeof(DataGrid), typeof(DataGridColumnHeader), typeof(GridViewColumnHeader),
                    typeof(GroupBox), typeof(Label), typeof(CheckBox), typeof(RadioButton),
                    typeof(Menu), typeof(MenuItem), typeof(ToolBarTray), typeof(ToolBar),
                    typeof(StatusBar), typeof(ScrollViewer), typeof(GridSplitter),
                    typeof(Calendar), typeof(ProgressBar)
                };
                foreach (var key in keys)
                    Resources.Remove(key);
            }
        }

        /// <summary>
        /// Создаёт стиль для указанного типа элемента с заданными свойствами.
        /// </summary>
        private Style MakeStyle(Type targetType, params Setter[] setters)
        {
            var style = new Style(targetType);
            foreach (var s in setters)
                style.Setters.Add(s);
            return style;
        }

        /// <summary>
        /// Обработчик пункта меню "Тёмная тема".
        /// Применяет тёмную тему и синхронизирует ToggleButton.
        /// </summary>
        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme(true);
            ThemeToggle.IsChecked = true;
        }

        /// <summary>
        /// Обработчик пункта меню "Светлая тема".
        /// Применяет светлую тему и синхронизирует ToggleButton.
        /// </summary>
        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme(false);
            ThemeToggle.IsChecked = false;
        }

        /// <summary>
        /// Обработчик ToggleButton "Тема" на панели инструментов.
        /// Переключает между тёмной и светлой темой.
        /// </summary>
        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme(ThemeToggle.IsChecked == true);
        }

        /// <summary>
        /// Обработчик кнопки "Загрузить аватар".
        /// Открывает диалог выбора файла и устанавливает выбранное изображение в качестве аватара.
        /// </summary>
        private void LoadAvatar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите изображение",
                Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp|Все файлы|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                AvatarImage.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        /// <summary>
        /// Обработчик ToggleButton "Режим редактирования".
        /// Скрывает или показывает правую панель.
        /// </summary>
        private void EditModeToggle_Click(object sender, RoutedEventArgs e)
        {
            if (EditModeToggle.IsChecked == true)
            {
                RightPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                RightPanel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Обработчик RepeatButton " - ".
        /// Уменьшает значение на 1 (минимум 0).
        /// </summary>
        private void DecreaseValue_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(RepeatValueText.Text);
            if (value > 0)
                RepeatValueText.Text = (value - 1).ToString();
        }

        /// <summary>
        /// Обработчик RepeatButton " + ".
        /// Увеличивает значение на 1 (максимум 100).
        /// </summary>
        private void IncreaseValue_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(RepeatValueText.Text);
            if (value < 100)
                RepeatValueText.Text = (value + 1).ToString();
        }

        /// <summary>
        /// Обработчик пункта меню "Выход".
        /// Завершает работу приложения.
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
