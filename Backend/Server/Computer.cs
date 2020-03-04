using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Server
{
    class Computer : INotifyPropertyChanged
    {
        private string name;
        private string updtime;
       

        public int Id { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string UpdTime
        {
            get { return updtime; }
            set
            {
                updtime = value;
                OnPropertyChanged("UpdTime");
            }
        }
       
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
