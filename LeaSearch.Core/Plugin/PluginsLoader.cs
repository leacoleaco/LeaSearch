﻿using LeaSearch.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LeaSearch.Infrastructure.Storage;
using Newtonsoft.Json;

namespace LeaSearch.Core.Plugin
{
    public class PluginsLoader
    {
        public const string Python = "python";
        public const string PythonExecutable = "pythonw.exe";


        public List<Plugin> LoadPlugins(string pluginInstallPath)
        {
            var pluginBases = ReadPluginBase(pluginInstallPath);
            if (pluginBases != null)
            {
                return LoadPluginsWithPluginBase(pluginBases);
            }

            return null;
        }

        /// <summary>
        /// read the plugin's baseinfo and custom setting
        /// </summary>
        /// <param name="pluginInstallPath">the plugin's parent dir</param>
        /// <returns></returns>
        private List<PluginBase> ReadPluginBase(string pluginInstallPath)
        {
            var result = new List<PluginBase>();
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

                        var pluginBase = new PluginBase(d.FullName, pluginMetadata, pluginsetting);
                        result.Add(pluginBase);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// load plugin from assembly
        /// </summary>
        /// <param name="pluginBase"></param>
        /// <returns></returns>
        private List<Plugin> LoadPluginsWithPluginBase(List<PluginBase> pluginBase)
        {
            var csharpPlugins = new List<Plugin>();
            pluginBase.ForEach(p =>
            {
                var plugin = LoadCSharpPlugin(p);
                csharpPlugins.Add(plugin);
            });

            //var pythonPlugins = PythonPlugins(metadatas, settings.PythonDirectory);
            //var executablePlugins = ExecutablePlugins(metadatas);
            //var plugins = csharpPlugins.Concat(pythonPlugins).Concat(executablePlugins).ToList();
            var plugins = csharpPlugins;
            return plugins;
        }

        /// <summary>
        /// load C# plugin,create instance
        /// </summary>
        /// <param name="pluginBase"></param>
        /// <returns></returns>
        private Plugin LoadCSharpPlugin(PluginBase pluginBase)
        {

#if DEBUG
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(pluginBase.PluginEntryPath));
            var types = assembly.GetTypes();
            var type = types.First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(IPlugin)));
            var pluginInstance = (IPlugin)Activator.CreateInstance(type);
#else
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(AssemblyName.GetAssemblyName(pluginBase.PluginEntryPath));
            }
            catch (Exception e)
            {
                Logger.Exception(
                    $"|PluginsLoader.CSharpPlugins|Couldn't load assembly for {pluginBase.PluginMetadata.Name}", e);
                return null;
            }
            var types = assembly.GetTypes();
            Type type;
            try
            {
                type = types.First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(IPlugin)));
            }
            catch (InvalidOperationException e)
            {
                Logger.Exception(
                    $"|PluginsLoader.CSharpPlugins|Can't find class implement IPlugin for <{pluginBase.PluginMetadata.Name}>",
                    e);
                return null;
            }
            IPlugin pluginInstance;
            try
            {
                pluginInstance = (IPlugin)Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                Logger.Exception(
                    $"|PluginsLoader.CSharpPlugins|Can't create instance for <{pluginBase.PluginMetadata.Name}>", e);
                return null;
            }
#endif
            return new Plugin(pluginBase, pluginInstance);
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

