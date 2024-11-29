using System.Windows;

namespace ContactsBook
{
    public partial class ContactWindow : Window
    {
        public Contact Contact { get; private set; }
        public ContactWindow(Contact contact)
        {
            InitializeComponent();
            Contact = contact;
            DataContext = Contact;
        }

        void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}