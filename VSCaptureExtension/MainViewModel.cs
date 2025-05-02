using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VSCaptureExtension
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ICommand ClickAddCommand { get;}

        public MainViewModel()
        {
            showCaptureWindow = false;

            ClickAddCommand = new RelayCommand(() =>
            {
                ShowCaptureWindow = !ShowCaptureWindow;
                var captureUI = new CaptureUI();
                captureUI.DataContext = this;
                captureUI.Show();
            });
        }

        private bool showCaptureWindow;

        public bool ShowCaptureWindow
        {
            get { return showCaptureWindow; }
            set { showCaptureWindow = value; OnPropertyChanged(); }
        }


    }
}
