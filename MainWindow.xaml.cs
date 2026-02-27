using System;
using System.Collections.Generic;
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
        /// Инициализирует новый экземпляр класса <see cref="MainWindow"/>.
        /// Загружает компоненты пользовательского интерфейса.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
