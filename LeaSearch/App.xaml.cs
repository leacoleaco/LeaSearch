using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using LeaSearch.Helper;
using System.IO;
using Autofac;
using LeaSearch.Common.Env;
using LeaSearch.Core.HotKey;
using LeaSearch.Core.I18N;
using LeaSearch.Core.Image;
using LeaSearch.Core.Ioc;
using LeaSearch.Core.Plugin;
using LeaSearch.Core.QueryEngine;
using LeaSearch.Core.Theme;
using LeaSearch.Infrastructure.ErrorReport;
using LeaSearch.Infrastructure.Logger;
using LeaSearch.Infrastructure.Storage;
using LeaSearch.ViewModels;
using LeaSearch.Views;

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


        private ContainerBuilder _builder;
        private IContainer _container;



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

            //=====================================   Doing prepare things   =========================================================
            // regist unhandled exception that is UI Thread cause 
            RegisterDispatcherUnhandledException();

            //read settings form file
            LeaSearchJsonStorage<Settings> _storage = new LeaSearchJsonStorage<Settings>();
            var settings = _storage.Load();

            //init notify bar icon
            InitializeNotifyIcon();

            //// IOC regist
            _builder = new ContainerBuilder();
            _builder.Register(c => settings).As<Settings>().SingleInstance();

            _builder.RegisterType<ImageManager>().SingleInstance();

            _builder.RegisterType<ThemeManager>();
            _builder.RegisterType<InternationalizationManager>().SingleInstance();
            _builder.RegisterType<HotKeyManager>().SingleInstance();
            _builder.RegisterType<PluginManager>().SingleInstance();
            _builder.RegisterType<QueryEngine>().SingleInstance();

            _builder.RegisterType<ShellViewModel>().SingleInstance();
            _builder.RegisterType<SuggestionResultViewModel>().SingleInstance();
            _builder.RegisterType<SearchResultViewModel>().SingleInstance();
            _builder.RegisterType<DetailResultViewModel>().SingleInstance();
            _builder.RegisterType<ShellView>().SingleInstance();

            _container = _builder.Build();
            Ioc.SetContainer(_container);
            //// IOC regist end

            base.OnStartup(e);

            //doing things when exit
            RegisterExitEvents();

            //// initalize image manager to start better manage img or cache img
            Ioc.Reslove<ImageManager>().Initialize();

            //// load plugin before change language, because plugin language also needs be changed
            Ioc.Reslove<InternationalizationManager>().ChangeLanguage(settings.Language);

            //// main windows needs initialized before theme change because of blur settigns
            Ioc.ResloveUsingLifetime<ThemeManager>().ChangeTheme(settings.Theme);

            //// re-setup the global hotkey
            Ioc.Reslove<HotKeyManager>().RefreshGlobalHotkeyAction();


            //// re-setup the global hotkey
            Ioc.Reslove<PluginManager>().LoadPlugins();


            Ioc.Reslove<QueryEngine>().Init();


            //get the windows to init a shellview and show windows
            var shellview = Ioc.Reslove<ShellView>();
            shellview.DataContext = Ioc.Reslove<ShellViewModel>();
        }



        public void Dispose()
        {
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

            _notifyIcon.Icon = LeaSearch.Properties.Resources.app;
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
