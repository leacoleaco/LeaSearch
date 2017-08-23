using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using LeaSearch.Common.Env;

namespace LeaSearch.Common.ViewModel
{
    public abstract class BaseViewModel : ViewModelBase
    {
        public Settings _settings;

        protected BaseViewModel(Settings settings)
        {
            _settings = settings;
        }


    }
}
