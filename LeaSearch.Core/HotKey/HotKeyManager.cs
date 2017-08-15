using System;
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


        public void SetHotkey(Hotkey hotkey, EventHandler<HotkeyEventArgs> action)
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
    }
}
