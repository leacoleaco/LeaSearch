using System;
using System.Windows;
using System.Windows.Threading;

namespace LeaSearch.Infrastructure.Dispatcher
{
    public static class DispatcherHelper
    {
        public static void BeginInvoke(Delegate method)
        {
            Application.Current.Dispatcher.BeginInvoke(method);
        }

        public static void BeginInvoke(Delegate method, params object[] args)
        {
            Application.Current.Dispatcher.BeginInvoke(method, args);
        }

        public static void BeginInvoke(DispatcherPriority priority, Delegate method, params object[] args)
        {
            Application.Current.Dispatcher.BeginInvoke(priority, method, args);
        }
    }
}
