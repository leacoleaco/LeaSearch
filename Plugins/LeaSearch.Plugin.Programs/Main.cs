using System;
using System.Diagnostics;
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
                    Extra = SharedContext.SharedMethod.SerializeToJson(win32)
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

                });

            }

            return indexInfo;

        }

        public override QueryListResult Query(QueryParam queryParam)
        {
            var res = new QueryListResult();

            var searchRes = PluginApi.SearchDataItems(queryParam.Keyword);
            //var searchRes = PluginApi.GetAllDataItems();

            foreach (var item in searchRes)
            {
                //ExcutableInfo excutableInfo = SharedContext.SharedMethod.DeserializeFromJson<ExcutableInfo>(item.Extra);
                var result = new ResultItem
                {
                    Title = item.Name,
                    SubTitle = item.Tip,
                    //IconPath = item.IconPath,
                    IconBytes = item.IconBytes,
                    SelectedAction = e =>
                    {
                        //try
                        //{
                        //    Process.Start(excutableInfo.GetProcessStartInfo());
                        //}
                        //catch (Exception)
                        //{
                        //    //Silently Fail for now.. todo
                        //}
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
