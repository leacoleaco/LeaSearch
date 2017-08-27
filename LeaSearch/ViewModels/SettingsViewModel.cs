using LeaSearch.Common.Env;
using LeaSearch.Core.HotKey;
using LeaSearch.Core.I18N;
using LeaSearch.Core.Ioc;
using LeaSearch.Core.Theme;
using LeaSearch.Infrastructure.Storage;

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
