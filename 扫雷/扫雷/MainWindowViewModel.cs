using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MineClearance
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 剩余地雷
        /// </summary>
        private int _remainder = 0;
        public int Remainder
        {
            get { return _remainder; }
            set
            {
                _remainder = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Remainder"));
                }
            }
        }
    }
}
