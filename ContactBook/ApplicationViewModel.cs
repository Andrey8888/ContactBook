using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HelloApp
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        Contact selectedContact;

        IFileService fileService;
        IDialogService dialogService;

        public ObservableCollection<Contact> Contacts { get; set; }

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

        private string currentSortProperty = "Surname";
        private bool isAscending = true;

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

        public void SortContacts(string property)
        {
            if (property == currentSortProperty)
            {
                isAscending = !isAscending;
            }
            else
            {
                currentSortProperty = property;
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



