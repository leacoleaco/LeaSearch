using System;
using LeaSearch.Common.Env;
using LeaSearch.Core.Notice;
using LeaSearch.Infrastructure.Logger;
using NHotkey;
using NHotkey.Wpf;

namespace LeaSearch.Core.HotKey
{
    /// <summary>
    /// manage all things with hotkey
    /// </summary>
    public class HotKeyManager
    {
        private Settings _settings;

        public HotKeyManager(Settings settings)
        {
            _settings = settings;
            RefreshGlobalHotkeyAction();
        }

        public void RefreshGlobalHotkeyAction()
        {
            AddOrReplaceHotKey(new Hotkey(_settings.ActiveHotkey), (o, e) =>
            {
                OnWakeUpCommand();
            });
        }

        internal void AddOrReplaceHotKey(Hotkey hotkey, EventHandler<HotkeyEventArgs> action)
        {
            string hotkeyStr = hotkey.ToString();
            try
            {
                HotkeyManager.Current.AddOrReplace(hotkeyStr, hotkey.CharKey, hotkey.ModifierKeys, action);
            }
            catch (Exception e)
            {
                Logger.Exception(e.Message, e);
                UiNoticeHelper.ShowMessageWithInternation("registerHotkeyFailed", hotkeyStr);
            }
        }


        public event Action WakeUpCommand;

        protected virtual void OnWakeUpCommand()
        {
            WakeUpCommand?.Invoke();
        }
    }
}
