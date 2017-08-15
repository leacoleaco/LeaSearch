using System;
using System.Windows.Input;
using LeaSearch.Core.I18N;
using LeaSearch.Core.Notice;
using NHotkey;
using NHotkey.Wpf;

namespace LeaSearch.Core.HotKey
{

    /// <summary>
    /// manage all things with hotkey
    /// </summary>
    public class HotKeyManager
    {

        private static void AddOrReplaceHotKey(string hotkeyStr, EventHandler<HotkeyEventArgs> action)
        {
            AddOrReplaceHotKey(new Hotkey(hotkeyStr), action);
        }

        private static void AddOrReplaceHotKey(Hotkey hotkey, EventHandler<HotkeyEventArgs> action)
        {
            string hotkeyStr = hotkey.ToString();
            try
            {
                HotkeyManager.Current.AddOrReplace(hotkeyStr, hotkey.CharKey, hotkey.ModifierKeys, action);
            }
            catch (Exception)
            {
                MessageUiHelper.ShowMessage("registerHotkeyFailed", hotkeyStr);
            }
        }

        /// <summary>
        /// get the Hotkey Instance
        /// </summary>
        public static HotKeyManager Instance { get; } = new HotKeyManager();

        #region setup action

        /// <summary>
        /// set wake up key
        /// </summary>
        /// <param name="hotkeyStr"></param>
        public void SetupWakeUpKey(string hotkeyStr)
        {
            AddOrReplaceHotKey(hotkeyStr, (o, e) =>
            {
                OnHotKeyWakeUpHandler();
            });
        }

        #endregion

        //use action is just to convenient and decoupe 
        #region Hotkey Action Event

        public event Action OnHotKeyWakeUp;

        protected virtual void OnHotKeyWakeUpHandler()
        {
            OnHotKeyWakeUp?.Invoke();
        }

        #endregion

    }


}
