using System.Threading.Tasks;

namespace ContactsBook
{
    public class EditContactCommand : AsyncRelayCommand
    {
        public EditContactCommand(ViewModel viewModel)
            : base(async (o) => await Execute(viewModel, o))
        {
            this.viewModel = viewModel;
        }

        private static async Task Execute(ViewModel viewModel, object selectedItem)
        {
            Contact contact = selectedItem as Contact;
            if (contact == null) return;

            Contact vm = new Contact
            {
                Id = contact.Id,
                Surname = contact.Surname,
                Name = contact.Name,
                Patronymic = contact.Patronymic,
                PlaceOfWork = contact.PlaceOfWork,
                PhoneNumber = contact.PhoneNumber
            };

            ContactWindow contactWindow = new ContactWindow(vm);
            if (contactWindow.ShowDialog() == true)
            {
                using (var dbContext = new ApplicationContext())
                {
                    var contactToUpdate = await dbContext.Contacts.FindAsync(contact.Id);
                    if (contactToUpdate != null)
                    {
                        contactToUpdate.Surname = contactWindow.Contact.Surname;
                        contactToUpdate.Name = contactWindow.Contact.Name;
                        contactToUpdate.Patronymic = contactWindow.Contact.Patronymic;
                        contactToUpdate.PlaceOfWork = contactWindow.Contact.PlaceOfWork;
                        contactToUpdate.PhoneNumber = contactWindow.Contact.PhoneNumber;

                        await dbContext.SaveChangesAsync();
                    }

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
