using LeaSearch.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using LeaSearch.Infrastructure.Logger;
using LeaSearch.SearchEngine;
using Newtonsoft.Json;

namespace LeaSearch.Core.Plugin
{
    public class PluginsLoader
    {
        public const string Python = "python";
        public const string PythonExecutable = "pythonw.exe";

        private readonly SharedContext _sharedContext;
        private LuceneManager _luceneManager;

        public PluginsLoader(SharedContext sharedContext, LuceneManager luceneManager)
        {
            _sharedContext = sharedContext;
            _luceneManager = luceneManager;
        }

        public List<Plugin> LoadPlugins(string pluginInstallPath)
        {
            var pluginBases = ReadPluginBaseInfo(pluginInstallPath);
            if (pluginBases != null)
            {
                return LoadPluginsWithPluginBaseInfo(pluginBases);
            }

            return null;
        }

        /// <summary>
        /// read the plugin's baseinfo and custom setting
        /// </summary>
        /// <param name="pluginInstallPath">the plugin's parent dir</param>
        /// <returns></returns>
        private List<PluginBaseInfo> ReadPluginBaseInfo(string pluginInstallPath)
        {
            var result = new List<PluginBaseInfo>();
            //search plugin dir file, read "plugin.json"
            DirectoryInfo dir = new DirectoryInfo(pluginInstallPath);
            if (!dir.Exists) return null;
            DirectoryInfo[] dii = dir.GetDirectories();
            if (dii.Length == 0) return null;
            foreach (var d in dii)
            {
                if (d.Exists)
                {
                    var p = Path.Combine(d.FullName, "plugin.json");

                    if (File.Exists(p))
                    {
                        //here we read the plugin.json
                        var pluginMetadata = JsonConvert.DeserializeObject<PluginMetadata>(File.ReadAllText(p));

                        //TODO:how to read the plugin's custom setting
                        var pluginsetting = new PluginSettings();

                        var pluginBase = new PluginBaseInfo(d.FullName, pluginMetadata, pluginsetting);
                        result.Add(pluginBase);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// load plugin from assembly
        ///从plugin的基础信息来加载 plugin assembly
        /// </summary>
        /// <param name="pluginBaseInfos"></param>
        /// <returns></returns>
        private List<Plugin> LoadPluginsWithPluginBaseInfo(List<PluginBaseInfo> pluginBaseInfos)
        {
            var csharpPlugins = new List<Plugin>();


            pluginBaseInfos.ForEach(p =>
            {

                var plugin = LoadCSharpPlugin(p);
                if (plugin != null)
                {
                    //加载 C# plugin
                    csharpPlugins.Add(plugin);
                }
            });



            var plugins = csharpPlugins;
            return plugins;
        }


        /// <summary>
        /// load C# plugin,create instance
        /// </summary>
        /// <param name="pluginBaseInfo"></param>
        /// <param name="pluginApiaData"></param>
        /// <param name="pluginApi"></param>
        /// <returns></returns>
        private Plugin LoadCSharpPlugin(PluginBaseInfo pluginBaseInfo)
        {

#if DEBUG
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(pluginBaseInfo.PluginEntryPath));
            var types = assembly.GetTypes();
            var type = types.First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(LeaSearch.Plugin.IPlugin)));
            var pluginInstance = (LeaSearch.Plugin.Plugin)Activator.CreateInstance(type);
#else
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(AssemblyName.GetAssemblyName(pluginBaseInfos.PluginEntryPath));
            }
            catch (Exception e)
            {
                Logger.Exception(
                    $"|PluginsLoader.CSharpPlugins|Couldn't load assembly for {pluginBaseInfos.PluginMetadata.Name}", e);
                return null;
            }
            var types = assembly.GetTypes();
            Type type;
            try
            {
                type = types.First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(Plugin)));
            }
            catch (InvalidOperationException e)
            {
                Logger.Exception(
                    $"|PluginsLoader.CSharpPlugins|Can't find class implement Plugin for <{pluginBaseInfos.PluginMetadata.Name}>",
                    e);
                return null;
            }
            Plugin pluginInstance;
            try
            {
                pluginInstance = (Plugin)Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                Logger.Exception(
                    $"|PluginsLoader.CSharpPlugins|Can't create instance for <{pluginBaseInfos.PluginMetadata.Name}>", e);
                return null;
            }
#endif
            var csharpPlugin = new Plugin(pluginBaseInfo, pluginInstance, type.FullName, PluginType.Csharp);

            //与plugin共享api
            IPluginApi pluginApi = new PluginApiForCsharpPlugin(csharpPlugin, _luceneManager);

            return InitCSharpPlugin(csharpPlugin, pluginApi);
        }

        /// <summary>
        /// 初始化插件
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="pluginApi"></param>
        /// <returns></returns>
        private Plugin InitCSharpPlugin(Plugin plugin, IPluginApi pluginApi)
        {
            try
            {


                plugin.PluginInstance?.InitPlugin(this._sharedContext, pluginApi);
                return plugin;
            }
            catch (Exception e)
            {
                Logger.Error($"plugin <{plugin}> call init method throw error: {e.Message}");
#if DEBUG
                MessageBox.Show($"plugin <{plugin}> call init method throw error: {e.Message}");
#endif

                return null;
            }
        }
    }
}

//public static IEnumerable<Plugin PythonPlugins(List<PluginMetadata> source, string pythonDirecotry)
//{
//    var metadatas = source.Where(o => o.Language.ToUpper() == AllowedLanguage.Python);
//    string filename;

//    if (string.IsNullOrEmpty(pythonDirecotry))
//    {
//        var paths = Environment.GetEnvironmentVariable(PATH);
//        if (paths != null)
//        {
//            var pythonPaths = paths.Split(';').Where(p => p.ToLower().Contains(Python));
//            if (pythonPaths.Any())
//            {
//                filename = PythonExecutable;
//            }
//            else
//            {
//                Log.Error("|PluginsLoader.PythonPlugins|Python can't be found in PATH.");
//                return new List<PluginPair>();
//            }
//        }
//        else
//        {
//            Log.Error("|PluginsLoader.PythonPlugins|PATH environment variable is not set.");
//            return new List<PluginPair>();
//        }
//    }
//    else
//    {
//        var path = Path.Combine(pythonDirecotry, PythonExecutable);
//        if (File.Exists(path))
//        {
//            filename = path;
//        }
//        else
//        {
//            Log.Error("|PluginsLoader.PythonPlugins|Can't find python executable in <b ");
//            return new List<PluginPair>();
//        }
//    }
//    Constant.PythonPath = filename;
//    var plugins = metadatas.Select(metadata => new PluginPair
//    {
//        Plugin = new PythonPlugin(filename),
//        Metadata = metadata
//    });
//    return plugins;
//}

//public static IEnumerable<Plugin> ExecutablePlugins(IEnumerable<PluginMetadata> source)
//{
//    var metadatas = source.Where(o => o.Language.ToUpper() == AllowedLanguage.Executable);

//    var plugins = metadatas.Select(metadata => new PluginPair
//    {
//        Plugin = new ExecutablePlugin(metadata.ExecuteFilePath),
//        Metadata = metadata
//    });
//    return plugins;
//}

