using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin.SystemControlPanel
{
    public class Main : IPlugin
    {

        private SharedContext _sharedContext;
        private PluginMetaData _pluginMetaData;
        private List<ControlPanelItem> controlPanelItems = new List<ControlPanelItem>();
        private string iconFolder;
        private string fileType;



        public void InitPlugin(SharedContext sharedContext, PluginMetaData pluginMetaData)
        {
            _sharedContext = sharedContext;
            _pluginMetaData = pluginMetaData;

            controlPanelItems = ControlPanelList.Create(48);
            iconFolder = Path.Combine(pluginMetaData.PluginRootPath, @"Images\ControlPanelIcons\");
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

        public bool SuitableForSuggectionQuery(QueryParam queryParam)
        {
            return false;
        }

        public PluginCalledArg PluginCallActive(QueryParam queryParam)
        {
            return null;
        }

        public QueryListResult Query(QueryParam queryParam)
        {
            var res = new QueryListResult();

            foreach (var item in controlPanelItems)
            {
                var result = new ResultItem
                {
                    Title = item.LocalizedString,
                    SubTitle = item.InfoTip,
                    IconPath = Path.Combine(_pluginMetaData.PluginRootPath,
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


        public QueryDetailResult QueryDetail(ResultItem currentItem)
        {
            return null;
        }

        public HelpInfo GetHelpInfo(QueryParam queryParam)
        {
            return null;
        }
    }
}
