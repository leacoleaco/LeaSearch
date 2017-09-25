using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
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
            //var iconFolder = Path.Combine(pluginApi.PluginRootPath, @"Images\ControlPanelIcons\");
            //var fileType = ".bmp";

            //if (!Directory.Exists(iconFolder))
            //{
            //    Directory.CreateDirectory(iconFolder);
            //}

            PluginApi.RemoveIndex();

            var dataItems = new List<DataItem>();
            foreach (ControlPanelItem item in controlPanelItems)
            {
                //if (!File.Exists(iconFolder + item.GUID + fileType))
                //{
                //    item.Icon?.ToBitmap().Save(iconFolder + item.GUID + fileType);
                //}
                dataItems.Add(
                    new DataItem()
                    {
                        Name = item.LocalizedString,
                        //IconPath = Path.Combine(PluginApi.PluginRootPath,
                        //    @"Images\\ControlPanelIcons\\" + item.GUID + fileType),
                        IconBytes = SharedContext.SharedMethod.BitmapToBytes(item.Icon?.ToBitmap()),
                        Tip = item.InfoTip,
                        Extra = SharedContext.SharedMethod.SerializeToJson(item.ExcutableInfo),
                    });
            }
            PluginApi.AddDataItemToIndex(dataItems.ToArray());
        }


        public override QueryListResult Query(QueryParam queryParam)
        {
            var res = new QueryListResult();

            var searchRes = PluginApi.SearchDataItems(queryParam.Keyword);
            //var searchRes = PluginApi.GetAllDataItems();

            foreach (var item in searchRes)
            {
                ExcutableInfo excutableInfo = SharedContext.SharedMethod.DeserializeFromJson<ExcutableInfo>(item.Extra);
                var result = new ResultItem
                {
                    Title = item.Name,
                    SubTitle = item.Tip,
                    //IconPath = item.IconPath,
                    IconBytes = item.IconBytes,
                    SelectedAction = e =>
                    {
                        try
                        {
                            Process.Start(excutableInfo.GetProcessStartInfo());
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
