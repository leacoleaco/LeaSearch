using System.Windows.Input;
using LeaSearch.Common.Env;

namespace LeaSearch.ViewModel
{
    public class MainViewModel
    {
        private Settings _settings;

        public MainViewModel(Settings settings)
        {
            _settings = settings;
        }
        
        #region Private Fields

        #endregion



        #region Command

        public ICommand EscCommand { get; set; }
        public ICommand SelectNextItemCommand { get; set; }
        public ICommand SelectPrevItemCommand { get; set; }
        public ICommand SelectNextPageCommand { get; set; }
        public ICommand SelectPrevPageCommand { get; set; }
        public ICommand StartHelpCommand { get; set; }
        public ICommand LoadContextMenuCommand { get; set; }
        public ICommand LoadHistoryCommand { get; set; }
        public ICommand OpenResultCommand { get; set; }

        #endregion
    }
}