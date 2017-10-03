using System;
using System.Collections.Generic;
using System.Diagnostics;
using LeaSearch.Plugin.Index;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin.SystemControlPanel
{
    public class Main : Plugin
    {


        public override void InitPlugin(SharedContext sharedContext, IPluginApi pluginApi)
        {
            base.InitPlugin(sharedContext, pluginApi);

            pluginApi.SetIconFromEmbedResource("Images.ControlPanel.png");

        }

        public override IndexInfo InitIndex(IndexInfo indexInfo)
        {
            var controlPanelItems = ControlPanelList.Create(48);
            var dataItems = new List<DataItem>();
            foreach (ControlPanelItem item in controlPanelItems)
            {
                indexInfo.AddItem(
                    new DataItem()
                    {
                        Name = item.LocalizedString,
                        IconBytes = SharedContext.SharedMethod.BitmapToBytes(item.Icon?.ToBitmap()),
                        Tip = item.InfoTip,
                        Extra = SharedContext.SharedMethod.SerializeToJson(item.ExcutableInfo),
                    });
            }
            return indexInfo;
        }

        public override bool SuitableForSuggectionQuery(QueryParam queryParam)
        {
            return true;
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
