using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TheFactorM.Federation;
namespace TheFactorM.Fedearation
{
    public class GlobalLoading : INotifyPropertyChanged
    {
        private ProgressIndicator _mangoIndicator;
        private GlobalLoading()
        {
        }
        public void Initialize(PhoneApplicationFrame frame)
        {
            // If using AgFx:
            // DataManager.Current.PropertyChanged += OnDataManagerPropertyChanged;
            _mangoIndicator = new ProgressIndicator();
            frame.Navigated += OnRootFrameNavigated;
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Use in Mango to share a single progress indicator instance.            
            var ee = e.Content;
            var pp = ee as PhoneApplicationPage;
            if (pp != null)
            {
                pp.SetValue(SystemTray.ProgressIndicatorProperty, _mangoIndicator);
            }
        }

        private void OnDataManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsLoading" == e.PropertyName)
            {
                // if AgFx: IsDataManagerLoading = DataManager.Current.IsLoading;
                NotifyValueChanged();
            }
        }
        private static GlobalLoading _in;
        public static GlobalLoading Instance
        {
            get
            {
                if (_in == null)
                {
                    _in = new GlobalLoading();
                }

                return _in;
            }
        }

        public bool IsDataManagerLoading { get; set; }
        public bool ActualIsLoading
        {
            get
            {
                return IsLoading || IsDataManagerLoading;
            }
        }

        private int _loadingCount;
        public bool IsLoading
        {
            get
            {
                return _loadingCount > 0;
            }
            set
            {
                bool loading = IsLoading;
                if (value)
                {
                    ++_loadingCount;
                }
                else
                {
                    --_loadingCount;
                }
                NotifyValueChanged();
                RaisePropertyChanged("IsLoading");
            }
        }

        private void NotifyValueChanged()
        {
            if (_mangoIndicator != null)
            {
                // Based on crash reports it seems we might not always be on the foreground thread
                // causing an UnauthorizedAccessException and a app crash as result
                // therefore ensure we are running on the foreground thread before we update any property on the 
                // progress indicator
                DeviceContext.Current.RunOnForegroundThread(() =>
                {
                    _mangoIndicator.IsIndeterminate = _loadingCount > 0 || IsDataManagerLoading;
                    // for now, just make sure it's always visible.
                    if (_mangoIndicator.IsVisible == false)
                    {
                        _mangoIndicator.IsVisible = true;

                    }
                });
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
