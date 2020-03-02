using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
