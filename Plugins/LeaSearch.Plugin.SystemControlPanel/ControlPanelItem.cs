using System.Drawing;

namespace LeaSearch.Plugin.SystemControlPanel
{
    //from:https://raw.githubusercontent.com/CoenraadS/Windows-Control-Panel-Items
    internal class ControlPanelItem
    {

        internal ControlPanelItem(string newLocalizedString, string newInfoTip, string newGUID, ExcutableInfo excutableInfo, Icon newIcon)
        {
            LocalizedString = newLocalizedString;
            InfoTip = newInfoTip;
            ExcutableInfo = excutableInfo;
            Icon = newIcon;
            GUID = newGUID;
        }


        internal string LocalizedString { get; private set; }
        internal string InfoTip { get; private set; }
        internal string GUID { get; private set; }
        internal ExcutableInfo ExcutableInfo { get; private set; }
        internal Icon Icon { get; private set; }

    }
}