using System.Windows;
using System.Windows.Controls;

namespace ContactsBook
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext =
                new ViewModel(new DefaultDialogService(), new JsonFileService());
        }

    }
}