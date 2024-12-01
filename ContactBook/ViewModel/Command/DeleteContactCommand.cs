using System.Threading.Tasks;

namespace ContactsBook
{
    public class DeleteContactCommand : AsyncRelayCommand
    {
        public DeleteContactCommand(ViewModel viewModel)
            : base(async (o) => await Execute(viewModel, o))
        {
            this.viewModel = viewModel;
        }

        private static async Task Execute(ViewModel viewModel, object selectedItem)
        {
            Contact contact = selectedItem as Contact;
            if (contact == null) return;

            using (var dbContext = new ApplicationContext())
            {
                dbContext.Contacts.Remove(contact);
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
