using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LeaSearch.Common.Env;
using LeaSearch.Common.ViewModel;

namespace LeaSearch.ViewModels
{
    public class SuggestionResultViewModel : BaseViewModel
    {
        private Core.Plugin.Plugin _currentPlugin;
        private int _currentPluginIndex;
        public ObservableCollection<Core.Plugin.Plugin> Plugins { get; set; } = new ObservableCollection<Core.Plugin.Plugin>();

        public SuggestionResultViewModel(Settings settings) : base(settings)
        {
        }

        public int CurrentPluginIndex
        {
            get { return _currentPluginIndex; }
            set
            {
                _currentPluginIndex = value;
                RaisePropertyChanged();
            }
        }

        public Core.Plugin.Plugin CurrentPlugin
        {
            get { return _currentPlugin; }
            set
            {
                _currentPlugin = value;
                RaisePropertyChanged();
            }
        }

        public void SetPlugins(IEnumerable<Core.Plugin.Plugin> plugins)
        {
            Plugins.Clear();
            var enumerable = plugins as Core.Plugin.Plugin[] ?? plugins.ToArray();
            if (enumerable.Any())
            {
                foreach (var plugin in enumerable)
                {
                    Plugins.Add(plugin);
                }
            }
        }

        public void Clear()
        {
            Plugins.Clear();
        }

    }
}