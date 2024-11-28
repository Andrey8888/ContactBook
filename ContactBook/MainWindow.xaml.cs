using System.Windows;
using System.Windows.Controls;

namespace HelloApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext =
                new ApplicationViewModel(new DefaultDialogService(), new JsonFileService());
        }

        private void OnHeaderClick(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header && header.Tag != null)
            {
                string sortProperty = header.Tag.ToString();
                if (DataContext is ApplicationViewModel viewModel)
                {
                    viewModel.SortContacts(sortProperty); // Вызов метода сортировки
                }
            }
        }
    }
}