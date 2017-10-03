using System;
using System.Threading.Tasks;
using LeaSearch.Plugin.Index;
using LeaSearch.Plugin.Query;
using Wox.Plugin.Program;
using Wox.Plugin.Program.Programs;

namespace LeaSearch.Plugin.Programs
{
    public class Main : Plugin
    {
        private Settings _settings = new Settings();


        public override void InitPlugin(SharedContext sharedContext, IPluginApi pluginApi)
        {
            base.InitPlugin(sharedContext, pluginApi);

            pluginApi.SetIconFromEmbedResource("Images.program.png");
        }

        public override IndexInfo InitIndex(IndexInfo indexInfo)
        {
            //读取传统的win32程序
            Win32[] w = Win32.All(_settings);
            foreach (var win32 in w)
            {
                indexInfo.AddItem(new DataItem()
                {
                    Name = win32.Name,
                    Tip = win32.FullPath,
                    IconPath = win32.IcoPath,
                    Extra = SharedContext.SharedMethod.SerializeToJson(new ExtItem() { Win32 = win32 })
                });
            }


            UWP.Application[] u = { };
            var windows10 = new Version(10, 0);
            var support = Environment.OSVersion.Version.Major >= windows10.Major;
            if (support)
            {
                //读取win10 的UWP程序
                u = UWP.All();
            }
            else
            {
                u = new UWP.Application[] { };
            }
            foreach (var application in u)
            {
                indexInfo.AddItem(new DataItem()
                {
                    Name = application.DisplayName,
                    Tip = application.Package.FullName,
                    IconPath = application.LogoPath,
                    Extra = SharedContext.SharedMethod.SerializeToJson(new ExtItem() { Application = application }),
                });

            }

            return indexInfo;

        }

        public override QueryListResult Query(QueryParam queryParam)
        {
            var res = new QueryListResult();

            var searchRes = PluginApi.SearchDataItems(queryParam.Keyword);

            foreach (var item in searchRes)
            {
                ExtItem extItem = SharedContext.SharedMethod.DeserializeFromJson<ExtItem>(item.Extra);
                var result = new ResultItem
                {
                    Title = item.Name,
                    SubTitle = item.Tip,
                    IconPath = item.IconPath,
                    IconBytes = item.IconBytes,
                    SelectedAction = e =>
                    {
                        try
                        {
                            extItem.Run();
                        }
                        catch (Exception)
                        {
                            var message = $"Can't start: {item.Name}";
                            SharedContext.SharedMethod.ShowMessage(message);
                            return new StateAfterCommandInvoke() { ShowProgram = true };
                        }
                        return new StateAfterCommandInvoke() { ShowProgram = false };
                    }
                };
                res.AddResultItem(result);
            }

            return res;


        }

        public override bool SuitableForSuggectionQuery(QueryParam queryParam)
        {
            return true;
        }

    }
}
