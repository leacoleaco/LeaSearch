using System.ComponentModel;
using System.Runtime.CompilerServices;
using LeaSearch.Common.Env;

namespace LeaSearch.Common.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public Settings _settings;

        protected BaseViewModel(Settings settings)
        {
            _settings = settings;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
