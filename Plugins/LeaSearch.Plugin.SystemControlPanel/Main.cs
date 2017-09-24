using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin.SystemControlPanel
{
    public class Main : Plugin
    {


        public override void InitPlugin(SharedContext sharedContext, IPluginApi pluginApi)
        {
            base.InitPlugin(sharedContext, pluginApi);

            //return;

            var controlPanelItems = ControlPanelList.Create(48);
            var iconFolder = Path.Combine(pluginApi.PluginRootPath, @"Images\ControlPanelIcons\");
            var fileType = ".bmp";

            if (!Directory.Exists(iconFolder))
            {
                Directory.CreateDirectory(iconFolder);
            }

            PluginApi.RemoveIndex();

            var dataItems = new List<DataItem>();
            foreach (ControlPanelItem item in controlPanelItems)
            {
                if (!File.Exists(iconFolder + item.GUID + fileType))
                {
                    item.Icon?.ToBitmap().Save(iconFolder + item.GUID + fileType);
                }
                dataItems.Add(
                    new DataItem()
                    {
                        Name = item.LocalizedString,
                        IconPath = Path.Combine(PluginApi.PluginRootPath,
                            @"Images\\ControlPanelIcons\\" + item.GUID + fileType),
                        Tip = item.InfoTip,
                        Body = SharedContext.SharedMethod.SerializeToJson(item),
                    });
            }
            PluginApi.AddDataItemToIndex(dataItems.ToArray());
        }


        public override QueryListResult Query(QueryParam queryParam)
        {
            var res = new QueryListResult();

            //var searchRes = PluginApi.SearchDataItems(queryParam.Keyword);
            var searchRes = PluginApi.GetAllDataItems();

            foreach (var item in searchRes)
            {
                ControlPanelItem obj = SharedContext.SharedMethod.DeserializeFromJson<ControlPanelItem>(item.Body);
                var result = new ResultItem
                {
                    Title = item.Name,
                    SubTitle = item.Tip,
                    IconPath = item.IconPath,
                    SelectedAction = e =>
                    {
                        try
                        {
                            Process.Start(obj.ExecutablePath);
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
