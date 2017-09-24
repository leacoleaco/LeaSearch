using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin.SystemControlPanel
{
    public class Main : Plugin
    {
        private List<ControlPanelItem> controlPanelItems = new List<ControlPanelItem>();
        private string iconFolder;
        private string fileType;


        public override void InitPlugin(SharedContext sharedContext, IPluginApi pluginApi)
        {
            base.InitPlugin(sharedContext, pluginApi);

            controlPanelItems = ControlPanelList.Create(48);
            iconFolder = Path.Combine(pluginApi.PluginRootPath, @"Images\ControlPanelIcons\");
            fileType = ".bmp";

            if (!Directory.Exists(iconFolder))
            {
                Directory.CreateDirectory(iconFolder);
            }


            foreach (ControlPanelItem item in controlPanelItems)
            {
                if (!File.Exists(iconFolder + item.GUID + fileType))
                {
                    item.Icon?.ToBitmap().Save(iconFolder + item.GUID + fileType);
                }
            }
        }


        public override QueryListResult Query(QueryParam queryParam)
        {
            var res = new QueryListResult();

            foreach (var item in controlPanelItems)
            {
                var result = new ResultItem
                {
                    Title = item.LocalizedString,
                    SubTitle = item.InfoTip,
                    IconPath = Path.Combine(PluginApi.PluginRootPath,
                        @"Images\\ControlPanelIcons\\" + item.GUID + fileType),
                    SelectedAction = e =>
                    {
                        try
                        {
                            Process.Start(item.ExecutablePath);
                        }
                        catch (Exception)
                        {
                            //Silently Fail for now.. todo
                        }
                        return new StateAfterCommandInvoke() { ShowProgram = false };
                    }
                };
                res.AddResultItem(result);
            }

            //List<Result> panelItems = results.OrderByDescending(o => o.Score).Take(5).ToList();

            return res;


        }


    }
}
