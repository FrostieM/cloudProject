﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Server
{
    class Program : INotifyPropertyChanged
    {
        private string name;
        private string version;
        private int idc;

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
        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }
        public int IdC
        {
            get { return idc; }
            set
            {
                idc = value;
                OnPropertyChanged("IdC");
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
