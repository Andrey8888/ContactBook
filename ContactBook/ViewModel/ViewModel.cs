using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactsBook
{
    public class ViewModel : INotifyPropertyChanged
    {
        private Contact selectedContact;

        public Contact SelectedContact
        {
            get => selectedContact;
            set
            {
                selectedContact = value;
                OnPropertyChanged(nameof(SelectedContact));
            }
        }

        private readonly IFileService fileService;
        private readonly IDialogService dialogService;

        private readonly ApplicationContext db = new ApplicationContext();

        public ObservableCollection<Contact> Contacts { get; set; }
        public ICollectionView FilteredContacts { get; }
        private string selectedFilterProperty;
        private string filterText;

        public string CurrentSortProperty { get; set; } = "Surname";
        public bool IsAscending { get; set; } = true;
        public Tuple<string, bool> SortDirection => Tuple.Create(CurrentSortProperty, IsAscending);

        public AddContactCommand AddCommand { get; }
        public EditContactCommand EditCommand { get; }
        public DeleteContactCommand DeleteCommand { get; }
        public SaveContactsCommand SaveCommand { get; }
        public OpenContactsCommand OpenCommand { get; }
        public SortContactsCommand SortCommand { get; }

        public ViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;

            db.Database.EnsureCreated();
            db.Contacts.Load();
            Contacts = db.Contacts.Local.ToObservableCollection();

            AddCommand = new AddContactCommand(this);
            EditCommand = new EditContactCommand(this);
            DeleteCommand = new DeleteContactCommand(this);
            SaveCommand = new SaveContactsCommand(this, dialogService, fileService);
            OpenCommand = new OpenContactsCommand(this, dialogService, fileService);
            SortCommand = new SortContactsCommand(this);

            FilteredContacts = CollectionViewSource.GetDefaultView(Contacts);
            FilteredContacts.Filter = FilterContacts;
            SelectedFilterProperty = "Surname";
        }

        public ObservableCollection<string> FilterProperties { get; set; } = new ObservableCollection<string>
        {
            "Surname", "Name", "Patronymic", "PlaceOfWork", "PhoneNumber"
        };

        public string SelectedFilterProperty
        {
            get => selectedFilterProperty;
            set
            {
                selectedFilterProperty = value;
                OnPropertyChanged(nameof(SelectedFilterProperty));
                ApplyFilter();
            }
        }

        private bool FilterContacts(object obj)
        {
            var contact = obj as Contact;
            if (contact == null) return false;

            if (string.IsNullOrEmpty(FilterText)) return true;

            var property = typeof(Contact).GetProperty(SelectedFilterProperty);
            if (property == null) return false;

            var value = property.GetValue(contact)?.ToString();
            return value != null && value.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public string FilterText
        {
            get => filterText;
            set
            {
                filterText = value;
                OnPropertyChanged(nameof(FilterText));
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            FilteredContacts.Refresh();
        }

        public object GetPropertyValue(Contact contact, string property)
        {
            var propInfo = typeof(Contact).GetProperty(property);
            return propInfo?.GetValue(contact);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}