using LeaSearch.Common.Env;
using LeaSearch.Core.HotKey;
using LeaSearch.Core.I18N;
using LeaSearch.Core.Theme;
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


            //// load plugin before change language, because plugin language also needs be changed
            InternationalizationManager.Instance.ChangeLanguage(Settings.Language);



            //// main windows needs initialized before theme change because of blur settigns
            ThemeManager.Instance.Settings = Settings;
            ThemeManager.Instance.ChangeTheme(Settings.Theme);


            //// setup the wakeup hotkey
            HotKeyManager.Instance.SetupWakeUpKey(Settings.ActiveHotkey);

            //Http.Proxy = _settings.Proxy;
            
        }
    }
}
