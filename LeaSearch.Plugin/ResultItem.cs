using System;
using System.Windows.Media;

namespace LeaSearch.Plugin
{

    /// <summary>
    /// return list item info
    /// </summary>
    public class ResultItem
    {
        public string PluginId { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string IconPath { get; set; }

        /// <summary>
        /// choose after action
        /// return true to hide leasearch after selected 
        /// </summary>
        public Func<SharedContext,bool> SelectedAction { get; set; }
    }
}