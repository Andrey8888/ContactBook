using System.Linq;
using System.Threading.Tasks;

namespace ContactsBook
{
    public class SortContactsCommand : AsyncRelayCommand
    {
        public SortContactsCommand(ViewModel viewModel)
            : base(async (o) => await Execute(viewModel, o))
        {
            this.viewModel = viewModel;
        }

        private static Task Execute(ViewModel viewModel, object parameter)
        {
            string property = parameter as string;
            if (string.IsNullOrEmpty(property)) return Task.CompletedTask;

            if (property == viewModel.CurrentSortProperty)
            {
                viewModel.IsAscending = !viewModel.IsAscending;
            }
            else
            {
                viewModel.CurrentSortProperty = property;
                viewModel.IsAscending = true;
            }

            var sortedContacts = viewModel.IsAscending
                ? viewModel.Contacts.OrderBy(c => viewModel.GetPropertyValue(c, property)).ToList()
                : viewModel.Contacts.OrderByDescending(c => viewModel.GetPropertyValue(c, property)).ToList();

            viewModel.Contacts.Clear();
            foreach (var contact in sortedContacts)
            {
                viewModel.Contacts.Add(contact);
            }

            viewModel.OnPropertyChanged(nameof(ViewModel.SortDirection));

            return Task.CompletedTask;
        }
    }
}
