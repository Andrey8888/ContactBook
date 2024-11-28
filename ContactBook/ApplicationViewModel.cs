using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace HelloApp
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        Contact selectedContact;

        IFileService fileService;
        IDialogService dialogService;

        public ObservableCollection<Contact> Contacts { get; set; }

        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
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

            return (contact.Surname != null && contact.Surname.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (contact.Name != null && contact.Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (contact.Patronymic != null && contact.Patronymic.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0);
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

        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      Contact contact = new Contact();
                      Contacts.Insert(0, contact);
                      SelectedContact = contact;
                  }));
            }
        }

        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ??
                  (removeCommand = new RelayCommand(obj =>
                  {
                      Contact contact = obj as Contact;
                      if (contact != null)
                      {
                          Contacts.Remove(contact);
                      }
                  },
                 (obj) => Contacts.Count > 0));
            }
        }
        private RelayCommand doubleCommand;
        public RelayCommand DoubleCommand
        {
            get
            {
                return doubleCommand ??
                  (doubleCommand = new RelayCommand(obj =>
                  {
                      Contact contact = obj as Contact;
                      if (contact != null)
                      {
                          Contact contactCopy = new Contact
                          {
                              Surname = contact.Surname,
                              Name = contact.Name,
                              Patronymic = contact.Patronymic
                          };
                          Contacts.Insert(0, contactCopy);
                      }
                  }));
            }
        }

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

        public (string, bool) SortDirection => (currentSortProperty, isAscending);

        public string CurrentSortProperty
        {
            get => currentSortProperty;
            set
            {
                currentSortProperty = value;
                OnPropertyChanged(nameof(CurrentSortProperty));
                OnPropertyChanged(nameof(SortDirection));
            }
        }

        public void SortContacts(string property)
        {
            if (property == currentSortProperty)
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
        }

        public class SortDirectionToBrushConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string sortProperty = parameter as string;
                if (value is Tuple<string, bool> currentSort && currentSort.Item1 == sortProperty)
                {
                    return currentSort.Item2 ? Brushes.Green : Brushes.Red;
                }
                return Brushes.Gray;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
        }

        public class SortPropertyToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string sortProperty = parameter as string;
                return value?.ToString() == sortProperty ? Visibility.Visible : Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
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

        public ApplicationViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;

            Contacts = new ObservableCollection<Contact>
            {
                new Contact {Surname="Петренко", Name="Илья", Patronymic="Алексеевич" },
                new Contact {Surname="Коробочкин", Name="Илья", Patronymic ="Ильич" },
                new Contact {Surname="Гаврилов", Name="Сергей", Patronymic="Федерович" },
                new Contact {Surname="Иванов", Name="Василий", Patronymic="Петрович" }
            };

            FilteredContacts = CollectionViewSource.GetDefaultView(Contacts);
            FilteredContacts.Filter = FilterContacts;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class RelayCommand : ICommand
    {
        Action<object?> execute;
        Func<object?, bool>? canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }
        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}



