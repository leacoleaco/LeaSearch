using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LeaSearch.ViewModels
{
    public class SuggestionResultViewModel
    {
        public ObservableCollection<Core.Plugin.Plugin> Plugins { get; set; } = new ObservableCollection<Core.Plugin.Plugin>();


        public void SetPlugins(List<Core.Plugin.Plugin> plugins)
        {
            Plugins.Clear();
            if (plugins.Any())
            {
                plugins.ForEach(p =>
                {
                    Plugins.Add(p);
                });
            }

        }

        public void Clear()
        {

        }
    }
}