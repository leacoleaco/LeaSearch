using LeaSearch.Common.Env;

namespace LeaSearch.ViewModels
{
    public class SettingsViewModel
    {
        /// <summary>
        /// Global settings
        /// </summary>
        private Settings _settings;


      

        public SettingsViewModel()
        {
        }


        public void Save()
        {
        }

        /// <summary>
        /// make settings work on system
        /// </summary>
        public void TakeEffectAllSettings()
        {
            //PluginManager.LoadPlugins(_settings.PluginSettings);

            //API = new PublicAPIInstance(_settingsVM, _mainViewModel);
            //PluginManager.InitializePlugins(API);
            //Log.Info($"|App.OnStartup|Dependencies Info:{ErrorReporting.DependenciesInfo()}");

            //Current.MainWindow = window;
            //Current.MainWindow.Title = Constant.Wox;


           

            //Http.Proxy = _settings.Proxy;

        }
    }
}
