using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Media.Imaging;
using EnvDTE80;
using VisionTest.VSExtension.Services;
using System.Threading;


namespace VisionTest.VSExtension
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int captureDelay = 0;
        private Rectangle screenShotZone;
        private bool showCaptureUI = false;
        private bool showCaptureTool = false;
        private BitmapImage currentScreenshot = null;
        private string textFound = string.Empty;
        private readonly InteropService interop = new InteropService();
        private string currentElementName = string.Empty;

        
        public string CurrentElementName
        {
            get { return currentElementName; }
            set
            {
                if (currentElementName == value) return;
                currentElementName = value;
                OnPropertyChanged();
            }
        }

        public string TextFound
        {
            get { return textFound; }
            set
            {
                if (textFound == value) return;
                textFound = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage CurrentScreenShot
        {
            get { return currentScreenshot; }
            set
            {
                if (currentScreenshot == value) return;
                currentScreenshot = value;
                OnPropertyChanged();

                if (currentScreenshot != null)
                {
                    TextFound = interop.GetText(currentScreenshot);
                }
            }
        }

        public bool ShowCaptureTool
        {
            get { return showCaptureTool; }
            set
            {
                if (showCaptureTool == value) return;
                showCaptureTool = value;
                OnPropertyChanged();
            }
        }

        public bool ShowCaptureUI
        {
            get { return showCaptureUI; }
            set
            {
                showCaptureUI = value;
                OnPropertyChanged();
            }
        }

        public Rectangle ScreenShotZone
        {
            get { return screenShotZone; }
            set 
            {
                ShowCaptureTool = false;
                screenShotZone = value;
                OnPropertyChanged();
                Thread.Sleep(100);
                CurrentScreenShot = Screen.Shoot(screenShotZone);
                Thread.Sleep(100);
                ShowCaptureUI = true;
            }
        }

        public int CaptureDelay
        {
            get { return captureDelay; }
            set 
            {
                if (captureDelay == value) return;
                captureDelay = value; 
                OnPropertyChanged(); 
            }
        }


        public ICommand ClickAddCommand { get; } //TODO do not initialize in the constructor
        public ICommand ClickNewCommand { get; }
        public ICommand ValidateCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RefreshCommand { get; }


        public MainViewModel(DTE2 dte)
        {
            ProjectService.Dte = dte;

            ClickAddCommand = new RelayCommand(() =>
            {
                ShowCaptureUI = true;
            });

            ClickNewCommand = new RelayCommand(() =>
            {
                ShowCaptureUI = false;
                Thread.Sleep(captureDelay * 1000);
                ShowCaptureTool = true;
            });

            ValidateCommand = new RelayCommand(() =>
            {
                ShowCaptureUI = false;
                ShowCaptureTool = false;
            });

            RefreshCommand = new RelayCommand(() =>
            {
                interop.UpdateEnum();
            });

            SaveCommand = new RelayCommand(() =>
            {
                try
                {
                    interop.Add(currentScreenshot, currentElementName);
                    ShowCaptureUI = false;
                    ShowCaptureTool = false;
                    CurrentScreenShot = null;
                    CurrentElementName = string.Empty;
                    TextFound = string.Empty;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
