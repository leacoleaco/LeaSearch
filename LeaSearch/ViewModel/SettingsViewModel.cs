using LeaSearch.Common.Env;
using LeaSearch.Infrastructure.Storage;

namespace LeaSearch.ViewModel
{
    public class SettingsViewModel
    {
        /// <summary>
        /// Global settings
        /// </summary>
        public Settings Settings { get; set; } = new Settings();


        private readonly LeaSearchJsonStorage<Settings> _storage;

        public SettingsViewModel()
        {
            _storage = new LeaSearchJsonStorage<Settings>();
            Settings = _storage.Load();
        }

        public void Save()
        {
            _storage.Save();
        }
    }
}
