using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private RelayCommand addCommand;

        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((o) =>
                  {
                      ContactWindow contactWindow = new ContactWindow(new Contact());
                      if (contactWindow.ShowDialog() == true)
                      {
                          Contact contact = contactWindow.Contact;

                  if (string.IsNullOrWhiteSpace(contact.Surname) ||
                          string.IsNullOrWhiteSpace(contact.Name) ||
                          string.IsNullOrWhiteSpace(contact.Patronymic))
                          {
                              System.Windows.MessageBox.Show("Поля 'Фамилия', 'Имя', 'Отчество' и 'Номер телефона' обязательны для заполнения.",
                                              "Ошибка",
                                              MessageBoxButton.OK,
                                              MessageBoxImage.Error);
                              return;
                          }

                          db.Contacts.Add(contact);
                          db.SaveChanges();
                      }
                  }));
            }
        }

        private RelayCommand editCommand;
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
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
                          contact.Surname = contactWindow.Contact.Surname;
                          contact.Name = contactWindow.Contact.Name;
                          contact.Patronymic = contactWindow.Contact.Patronymic;
                          contact.PlaceOfWork = contactWindow.Contact.PlaceOfWork;
                          contact.PhoneNumber = contactWindow.Contact.PhoneNumber;
                          db.Entry(contact).State = EntityState.Modified;
                          db.SaveChanges();
                      }
                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      Contact? contact = selectedItem as Contact;
                      if (contact == null) return;
                      db.Contacts.Remove(contact);
                      db.SaveChanges();
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

        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ??
                  (saveCommand = new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.SaveFileDialog() == true)
                          {
                              fileService.Save(dialogService.FilePath, Contacts.ToList());
                              dialogService.ShowMessage("Файл сохранен");
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.OpenFileDialog() == true)
                          {
                              var contacts = fileService.Open(dialogService.FilePath);
                              Contacts.Clear();
                              foreach (var p in contacts)
                                  Contacts.Add(p);
                              dialogService.ShowMessage("Файл открыт");
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }

        //private RelayCommand copyCommand;
        //public RelayCommand CopyCommand
        //{
        //    get
        //    {
        //        return copyCommand ??
        //          (copyCommand = new RelayCommand(obj =>
        //          {
        //              Contact contact = obj as Contact;
        //              if (contact != null)
        //              {
        //                  Contact contactCopy = new Contact
        //                  {
        //                      Surname = contact.Surname,
        //                      Name = contact.Name,
        //                      Patronymic = contact.Patronymic
        //                  };
        //                  Contacts.Insert(0, contactCopy);
        //              }
        //          }));
        //    }
        //}

        private RelayCommand sortCommand;
        public RelayCommand SortCommand
        {
            get
            {
                return sortCommand ?? (sortCommand = new RelayCommand(obj =>
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
