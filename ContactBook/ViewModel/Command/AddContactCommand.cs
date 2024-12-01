using System.Threading.Tasks;
using System.Windows;

namespace ContactsBook
{
    public class AddContactCommand : AsyncRelayCommand
    {
        public AddContactCommand(ViewModel viewModel)
            : base(async (o) => await Execute(viewModel))
        {
            this.viewModel = viewModel;
        }

        private static async Task Execute(ViewModel viewModel)
        {
            ContactWindow contactWindow = new ContactWindow(new Contact());
            if (contactWindow.ShowDialog() == true)
            {
                Contact contact = contactWindow.Contact;
                if (string.IsNullOrWhiteSpace(contact.Surname) || string.IsNullOrWhiteSpace(contact.Name) || string.IsNullOrWhiteSpace(contact.Patronymic) 
                    || string.IsNullOrWhiteSpace(contact.PlaceOfWork) || string.IsNullOrWhiteSpace(contact.PhoneNumber))
                {
                    MessageBox.Show("Все поля обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await Task.Delay(5000); // имитация задержки при добавление нового контакта 

                using (var dbContext = new ApplicationContext())
                {
                    dbContext.Contacts.Add(contact);
                    await dbContext.SaveChangesAsync();

                    viewModel.Contacts.Clear();
                    foreach (var c in dbContext.Contacts)
                    {
                        viewModel.Contacts.Add(c);
                    }
                }
            }
        }
    }
}
