using System.Collections.Generic;
using System.Collections.ObjectModel;
using LeaSearch.Plugin;

namespace LeaSearch.ViewModels
{
    public class SearchResultViewModel
    {
        public ObservableCollection<ResultItem> Results { get; } = new ObservableCollection<ResultItem>();

        public void Clear()
        {
            Results.Clear();
        }

        public void SetResults(IEnumerable<ResultItem> list)
        {
            Results.Clear();
            foreach (var resultItem in list)
            {
                Results.Add(resultItem);
            }
        }
    }
}