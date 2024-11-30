using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactsBook
{
    public class ViewModel : INotifyPropertyChanged
    {
        private Contact selectedContact;

        private IFileService fileService;
        private IDialogService dialogService;

        public ObservableCollection<Contact> Contacts { get; set; }

        private ApplicationContext db = new ApplicationContext();

        private AsyncRelayCommand addCommand;

        public AsyncRelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                    (addCommand = new AsyncRelayCommand(async (o) =>
                    {
                        ContactWindow contactWindow = new ContactWindow(new Contact());
                        if (contactWindow.ShowDialog() == true)
                        {
                            Contact contact = contactWindow.Contact;

                            if (string.IsNullOrWhiteSpace(contact.Surname) ||
                                string.IsNullOrWhiteSpace(contact.Name) ||
                                string.IsNullOrWhiteSpace(contact.Patronymic))
                            {
                                System.Windows.MessageBox.Show("Поля 'Фамилия', 'Имя', 'Отчество' обязательны для заполнения.",
                                                               "Ошибка",
                                                               MessageBoxButton.OK,
                                                               MessageBoxImage.Error);
                                return;
                            }

                            await Task.Delay(5000); // имитация задержки при добавление нового контакта 

                            using (var dbContext = new ApplicationContext())
                            {
                                dbContext.Contacts.Add(contact);
                                await dbContext.SaveChangesAsync();
                                Contacts.Clear();
                                foreach (var c in dbContext.Contacts)
                                {
                                    Contacts.Add(c);
                                }
                            }
                        }
                    }));
            }
        }

        private AsyncRelayCommand editCommand;
        public AsyncRelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                    (editCommand = new AsyncRelayCommand(async (selectedItem) =>
                    {
                        Contact? contact = selectedItem as Contact;
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

                                Contacts.Clear();
                                foreach (var c in dbContext.Contacts)
                                {
                                    Contacts.Add(c);
                                }
                            }
                        }
                    }));
            }
        }

        private AsyncRelayCommand deleteCommand;
        public AsyncRelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                    (deleteCommand = new AsyncRelayCommand(async (selectedItem) =>
                    {
                        Contact? contact = selectedItem as Contact;
                        if (contact == null) return;

                        using (var dbContext = new ApplicationContext())
                        {
                            dbContext.Contacts.Remove(contact);
                            await dbContext.SaveChangesAsync();

                            Contacts.Clear();
                            foreach (var c in dbContext.Contacts)
                            {
                                Contacts.Add(c);
                            }
                        }
                    }));
            }
        }

        private string selectedFilterProperty;
        private string filterText;

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

        public ICollectionView FilteredContacts { get; }

        private void ApplyFilter()
        {
            FilteredContacts.Refresh();
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

        private AsyncRelayCommand saveCommand;
        public AsyncRelayCommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new AsyncRelayCommand(async (obj) =>
                {
                    try
                    {
                        if (dialogService.SaveFileDialog() == true)
                        {
                            await Task.Run(() => fileService.Save(dialogService.FilePath, Contacts.ToList()));
                            dialogService.ShowMessage("Файл сохранен");
                        }
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage($"Ошибка при сохранении файла: {ex.Message}");
                    }
                }));
            }
        }

        private AsyncRelayCommand openCommand;
        public AsyncRelayCommand OpenCommand
        {
            get
            {
                return openCommand ?? (openCommand = new AsyncRelayCommand(async (obj) =>
                {
                    try
                    {
                        if (dialogService.OpenFileDialog() == true)
                        {
                            var contacts = await Task.Run(() => fileService.Open(dialogService.FilePath));

                            await App.Current.Dispatcher.InvokeAsync(() =>
                            {
                                Contacts.Clear();
                                foreach (var p in contacts)
                                {
                                    Contacts.Add(p);
                                }
                            });

                            dialogService.ShowMessage("Файл открыт");

                            using (var dbContext = new ApplicationContext())
                            {
                                dbContext.Contacts.RemoveRange(dbContext.Contacts);
                                dbContext.Contacts.AddRange(contacts);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage($"Ошибка при открытии файла: {ex.Message}");
                    }
                }));
            }
        }

        private AsyncRelayCommand sortCommand;
        public AsyncRelayCommand SortCommand
        {
            get
            {
                return sortCommand ?? (sortCommand = new AsyncRelayCommand(async (obj) =>
                {
                    string property = obj as string;
                    if (!string.IsNullOrEmpty(property))
                    {
                        SortContacts(property);
                    }
                }));
            }
        }

        private string currentSortProperty;
        private bool isAscending;

        public string CurrentSortProperty
        {
            get => currentSortProperty;
            set
            {
                currentSortProperty = value;
                OnPropertyChanged(nameof(CurrentSortProperty));
            }
        }

        public Tuple<string, bool> SortDirection => Tuple.Create(CurrentSortProperty, isAscending);

        public void SortContacts(string property)
        {
            if (property == CurrentSortProperty)
            {
                isAscending = !isAscending;
            }
            else
            {
                CurrentSortProperty = property;
                isAscending = true;
            }

            var sortedContacts = isAscending
                ? Contacts.OrderBy(c => GetPropertyValue(c, property)).ToList()
                : Contacts.OrderByDescending(c => GetPropertyValue(c, property)).ToList();

            Contacts.Clear();
            foreach (var contact in sortedContacts)
            {
                Contacts.Add(contact);
            }

            OnPropertyChanged(nameof(SortDirection));
        }

        private object GetPropertyValue(Contact contact, string property)
        {
            return typeof(Contact).GetProperty(property)?.GetValue(contact);
        }

        public Contact SelectedContact
        {
            get { return selectedContact; }
            set
            {
                selectedContact = value;
                OnPropertyChanged("SelectedContact");
            }
        }

        public ViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;

            db.Database.EnsureCreated();
            db.Contacts.Load();

            Contacts = db.Contacts.Local.ToObservableCollection();

            FilteredContacts = CollectionViewSource.GetDefaultView(Contacts);
            FilteredContacts.Filter = FilterContacts;

            SelectedFilterProperty = "Surname";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
