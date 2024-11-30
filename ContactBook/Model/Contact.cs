using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ContactsBook
{
    public class Contact : INotifyPropertyChanged
    {
        private string surname;
        private string name;
        private string patronymic;
        private string placeOfWork;
        private string phoneNumber;

        private Contact contact;

        public int Id { get; set; }
        public string Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                OnPropertyChanged("Surname");
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Patronymic
        {
            get { return patronymic; }
            set
            {
                patronymic = value;
                OnPropertyChanged("Patronymic");
            }
        }
        public string PlaceOfWork
        {
            get { return placeOfWork; }
            set
            {
                placeOfWork = value;
                OnPropertyChanged("PlaceOfWork");
            }
        }
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}