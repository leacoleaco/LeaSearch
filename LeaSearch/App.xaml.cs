using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using LeaSearch.Helper;
using Wox.Infrastructure.Logger;
using System.IO;
using LeaSearch.Infrastructure.ErrorReport;

namespace LeaSearch
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application, IDisposable, ISingleInstanceApp
    {
        private const string Unique = "LeaSearch_Unique_Application_String";

        /// <summary>
        /// global notifyIcon
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

        public App()
        {
            //regist exception handler when start program
            //Logger.Info($"|App.OnStartup|Runtime info:{ErrorReporting.RuntimeInfo()}");
            // regist excetion handler
            RegisterAppDomainExceptions();
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
            Logger.Info("App.OnStartup-----------------------------");


            // regist unhandled exception that is UI Thread cause 
            RegisterDispatcherUnhandledException();

            //init notify bar icon
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

            //doing things when exit
            RegisterExitEvents();

            //_mainVM.MainWindowVisibility = _settings.HideOnStartup ? Visibility.Hidden : Visibility.Visible;
            Logger.Info("|App.OnStartup-------------------------------  ");

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


        #region NotifyIcon 任务栏图标

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

        #endregion


        #region ErrorHandler

        /// <summary>
        /// register unhandled exception that is UI Thread cause
        /// </summary>
        //[Conditional("RELEASE")]
        private void RegisterDispatcherUnhandledException()
        {
            this.DispatcherUnhandledException += ErrorReporting.DispatcherUnhandledException;
        }


        /// <summary>
        /// register unhandled exception that is not UI thread cause
        /// </summary>
        //[Conditional("RELEASE")]
        private static void RegisterAppDomainExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += ErrorReporting.UnhandledExceptionHandle;


            //Occurs when an exception is thrown in managed code, before the runtime searches the call stack for an exception handler in the application domain.
            //https://msdn.microsoft.com/en-us/library/system.appdomain.firstchanceexception.aspx
            //AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
            //{
            //    Logger.Error("|App.RegisterAppDomainExceptions|First Chance Exception:\r\n" + e.Exception.StackTrace.ToString());
            //};
        }

        #endregion

    }
}
