using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MineClearance
{
    /// <summary>
    /// DifficultySelectionPopup.xaml 的交互逻辑
    /// </summary>
    public partial class DifficultySelectionPopup : Window
    {
        event Action<int, int> SaveDifficult;

        public DifficultySelectionPopup()
        {
            InitializeComponent();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //var aaa = radioButtons.
            //SaveDifficult?.Invoke();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class DifficultySelectionPopupViewModel : INotifyPropertyChanged
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
