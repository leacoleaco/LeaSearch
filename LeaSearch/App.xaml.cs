using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using LeaSearch.Helper;
using Wox.Infrastructure.Logger;
using System.IO;

namespace LeaSearch
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application, IDisposable, ISingleInstanceApp
    {
        private const string Unique = "LeaSearch_Unique_Application_String";

        /// <summary>
        /// globall notifyIcon
        /// </summary>
        private readonly System.Windows.Forms.NotifyIcon _notifyIcon = new System.Windows.Forms.NotifyIcon();

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        #region ISingleInstanceApp Members
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // handle command line arguments of second instance
            // ...
            return true;
        }
        #endregion

        /// <summary>
        /// When program start
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.Info("|App.OnStartup|Begin Wox startup ----------------------------------------------------");


            //Logger.Info($"|App.OnStartup|Runtime info:{ErrorReporting.RuntimeInfo()}");
            //RegisterAppDomainExceptions();
            //RegisterDispatcherUnhandledException();
            InitializeNotifyIcon();

            //_settingsVM = new SettingWindowViewModel();
            //_settings = _settingsVM.Settings;

            //PluginManager.LoadPlugins(_settings.PluginSettings);
            //_mainVM = new MainViewModel(_settings);
            //var window = new MainWindow(_settings, _mainVM);
            //API = new PublicAPIInstance(_settingsVM, _mainVM);
            //PluginManager.InitializePlugins(API);
            //Log.Info($"|App.OnStartup|Dependencies Info:{ErrorReporting.DependenciesInfo()}");

            //Current.MainWindow = window;
            //Current.MainWindow.Title = Constant.Wox;

            //// happlebao todo temp fix for instance code logic
            //// load plugin before change language, because plugin language also needs be changed
            //InternationalizationManager.Instance.Settings = _settings;
            //InternationalizationManager.Instance.ChangeLanguage(_settings.Language);
            //// main windows needs initialized before theme change because of blur settigns
            //ThemeManager.Instance.Settings = _settings;
            //ThemeManager.Instance.ChangeTheme(_settings.Theme);

            //Http.Proxy = _settings.Proxy;

            RegisterExitEvents();

            //_mainVM.MainWindowVisibility = _settings.HideOnStartup ? Visibility.Hidden : Visibility.Visible;
            Logger.Info("|App.OnStartup|End Wox startup ----------------------------------------------------  ");

            base.OnStartup(e);
        }


        public void Dispose()
        {
            // if sessionending is called, exit proverbially be called when log off / shutdown
            // but if sessionending is not called, exit won't be called when log off / shutdown
            //if (!_disposed)
            //{
            //    _mainVM.Save();
            //    _settingsVM.Save();

            //    PluginManager.Save();
            //    ImageLoader.Save();
            //    Alphabet.Save();

            //    _disposed = true;
            //}

            // dispose the notifyicon 
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }

        }


        private void RegisterExitEvents()
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Dispose();
            Current.Exit += (s, e) => Dispose();
            Current.SessionEnding += (s, e) => Dispose();
        }



        /// <summary>
        /// initialize notifyIcon
        /// 初始化 通知栏图标
        /// </summary>
        private void InitializeNotifyIcon()
        {
            var contextMenu1 = new System.Windows.Forms.ContextMenu();
            var menuItem1 = new System.Windows.Forms.MenuItem();

            // Initialize contextMenu1
            contextMenu1.MenuItems.AddRange(
                new System.Windows.Forms.MenuItem[] { menuItem1 });

            // Initialize menuItem1
            menuItem1.Index = 0;
            menuItem1.Text = "E&xit";
            menuItem1.Click += delegate
            {
                Application.Current.Shutdown();
            };

            _notifyIcon.Icon = LeaSearch.Properties.Resources.favicon;
            _notifyIcon.ContextMenu = contextMenu1;
            _notifyIcon.Visible = true;

        }

    }
}
