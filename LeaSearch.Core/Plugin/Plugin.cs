using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using LeaSearch.Common.Env;
using LeaSearch.Core.I18N;
using LeaSearch.Infrastructure.Logger;
using LeaSearch.Infrastructure.Storage;
using LeaSearch.Plugin;
using LeaSearch.Plugin.Index;
using LeaSearch.Plugin.Query;
using LeaSearch.Plugin.Setting;
using Newtonsoft.Json;

namespace LeaSearch.Core.Plugin
{
    /// <summary>
    /// this is the all things for a plugin
    /// 加载到主程序后的插件类
    /// 包含类插件进程的所有信息
    /// </summary>
    public class Plugin : PluginBaseInfo
    {

        private const string LanguageFolder = "Languages";


        public Plugin(PluginBaseInfo pluginBaseInfo, LeaSearch.Plugin.Plugin pluginInstance, string pluginId, PluginType pluginType, Assembly pluginAssembly) : base(pluginBaseInfo.PluginRootPath, pluginBaseInfo.PluginMetadata)
        {
            PluginInstance = pluginInstance;
            PluginId = pluginId;
            PluginType = pluginType;
            _pluginAssembly = pluginAssembly;
        }

        /// <summary>
        /// plugin 's instance
        /// </summary>
        private LeaSearch.Plugin.Plugin PluginInstance { get; }

        /// <summary>
        /// 插件assembly
        /// </summary>
        public Assembly PluginAssembly => _pluginAssembly;
        private readonly Assembly _pluginAssembly;

        /// <summary>
        /// 插件初始化信息
        /// </summary>
        public PluginInitInfo PluginInitInfo { get; } = new PluginInitInfo();

        /// <summary>
        /// 插件的资源
        /// </summary>
        public ResourceDictionary LanguageResourceDictionary { get; private set; }

        /// <summary>
        /// plugin's id
        /// </summary>
        public String PluginId { get; }

        /// <summary>
        /// 插件类型
        /// </summary>
        public PluginType PluginType { get; }


        /// <summary>
        /// if plugin is not correct load, or set to disabled, then it will disabled
        /// </summary>
        public bool IsDisabled => PluginMetadata == null || PluginSettings == null || PluginInstance == null || PluginSettings.Disabled;

        /// <summary>
        /// the mode that we call for this plugin 
        /// </summary>
        public QueryMode QueryMode { get; set; }


        public override string ToString()
        {
            if (PluginMetadata != null && string.IsNullOrEmpty(PluginMetadata.Name))
            {
                return PluginMetadata.Name;
            }

            return base.ToString();
        }

        /// <summary>
        /// 加载插件资源
        /// </summary>
        /// <param name="language"></param>
        public void LoadLanguage(Language language)
        {
            var stream = GetPluginEmbedResouceStream($"Languages.{language.LanguageCode}.xaml");
            if (stream == null) return;
            var resourceDictionary = XamlReader.Load(stream) as ResourceDictionary;
            if (resourceDictionary == null) return;
            LanguageResourceDictionary = resourceDictionary;
        }

        /// <summary>
        /// 读取插件中的文本资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetTranslation(string key)
        {
            var resource = LanguageResourceDictionary?[key];
            if (resource is string)
            {
                return resource.ToString();
            }

            Logger.Error($"|Internationalization.GetTranslation|No Translation for key {key}");
            return $"No Translation for key {key}";
        }

        /// <summary>
        /// 读取嵌入的资源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Stream GetPluginEmbedResouceStream(string name)
        {
            if (PluginAssembly == null) return null;
            var pluginName = PluginAssembly.GetName()?.Name;

            return PluginAssembly.GetManifestResourceStream($"{pluginName}.{name}");
        }

        #region pluginInstanceMethodCall

        public Plugin InvokeInitPlugin(SharedContext sharedContext, IPluginApi pluginApi)
        {

            try
            {
                PluginInstance?.InitPlugin(sharedContext, pluginApi);
                return this;
            }
            catch (Exception e)
            {
                Logger.Exception($"plugin <{PluginId}> call InitPlugin method throw error: {e.Message}", e);
#if DEBUG
                MessageBox.Show($"plugin <{PluginId}> call InitPlugin method throw error: {e.Message}");
#endif
                return null;
            }
        }

        public HelpInfo InvokeGetHelpInfo(QueryParam queryParam)
        {
            try
            {
                return PluginInstance.GetHelpInfo(queryParam);
            }
            catch (Exception e)
            {
                Logger.Exception($"plugin <{PluginId}> call GetHelpInfo error: {e.Message}", e);
#if DEBUG
                MessageBox.Show($"plugin <{PluginId}> call GetHelpInfo error: {e.Message}");
#endif
            }
            return null;
        }

        private bool InvokeCanReindexThisTime(DateTime lastIndexTime)
        {
            try
            {
                return PluginInstance.CanReindexThisTime(lastIndexTime);
            }
            catch (Exception e)
            {
                Logger.Exception($"plugin <{PluginId}> call CanReindexThisTime error: {e.Message}", e);
#if DEBUG
                MessageBox.Show($"plugin <{PluginId}> call CanReindexThisTime error: {e.Message}");
#endif
            }
            return false;
        }

        public IndexInfo InvokeInitIndex(IndexInfo indexInfo)
        {
            //无需更新缓存，则跳过
            if (PluginSettings.NeedUpdateIndex == -1) return null;
            if (PluginSettings.NeedUpdateIndex == 0 && !InvokeCanReindexThisTime(PluginSettings.LastIndexTime))
            {
                return null;
            }

            try
            {
                var ret = PluginInstance.InitIndex(new IndexInfo(PluginId));

                if (PluginSettings.NeedUpdateIndex == 1)
                {
                    PluginSettings.NeedUpdateIndex = 0;
                }

                PluginSettings.LastIndexTime = DateTime.Now;

                SavePluginSettings();

                return ret;
            }
            catch (Exception e)
            {
                Logger.Exception($"plugin <{PluginId}> call InitIndex method throw error: {e.Message}", e);
#if DEBUG
                MessageBox.Show($"plugin <{PluginId}> call InitIndex method throw error: {e.Message}");
#endif
            }
            return null;
        }

        public bool InvokeSuitableForSuggectionQuery(QueryParam queryParam, Action failureAction = null)
        {
            try
            {
                return PluginInstance.SuitableForSuggectionQuery(queryParam);

            }
            catch (Exception e)
            {
                Logger.Exception($"plugin <{PluginId}> call SuitableForThisQuery method throw error: {e.Message}", e);
#if DEBUG
                MessageBox.Show($"plugin <{PluginId}> call SuitableForThisQuery method throw error: {e.Message}");
#endif

                failureAction?.Invoke();
            }
            return false;
        }

        public PluginCalledArg InvokePluginCallActive(QueryParam queryParam, Action failureAction = null)
        {
            try
            {
                return PluginInstance.PluginCallActive(queryParam);
            }
            catch (Exception e)
            {
                Logger.Exception($"plugin <{PluginId}> call PluginCallActive  method throw error: {e.Message}", e);
#if DEBUG
                MessageBox.Show($"plugin <{PluginId}> call PluginCallActive method throw error: {e.Message}");
#endif
                failureAction?.Invoke();
            }
            return null;
        }

        public QueryListResult InvokeQueryList(QueryParam queryParam, Action failureAction = null)
        {

            try
            {
                return PluginInstance.QueryList(queryParam);
            }
            catch (Exception e)
            {
                Logger.Exception($"plugin <{PluginId}> call QueryList method throw error: {e.Message}", e);
#if DEBUG
                MessageBox.Show($"plugin <{PluginId}> call QueryList method throw error: {e.Message}");
#endif
                failureAction?.Invoke();
            }
            return null;
        }

        public QueryDetailResult InvokeQueryDetail(QueryParam queryParam, Action failureAction = null)
        {
            try
            {
                return PluginInstance.QueryDetail(queryParam);

            }
            catch (Exception e)
            {
                Logger.Exception($"plugin <{PluginId}> call QueryDetail method throw error: {e.Message}", e);
#if DEBUG
                MessageBox.Show($"plugin <{PluginId}> call QueryDetail method throw error: {e.Message}");
#endif
                failureAction?.Invoke();
            }
            return null;
        }

        public QueryItemDetailResult InvokeQueryItemDetail(ResultItem currentItem)
        {
            try
            {
                return PluginInstance.QueryItemDetail(currentItem);
            }
            catch (Exception e)
            {
                Logger.Exception($"plugin <{PluginId}> call QueryDetail method throw error: {e.Message}", e);
#if DEBUG
                MessageBox.Show($"plugin <{PluginId}> call QueryDetail method throw error: {e.Message}");
#endif
            }
            return null;
        }



        #endregion

    }

    public enum QueryMode
    {
        Suggection, PluginCall
    }



    public class PluginBaseInfo
    {

        private PluginSettingJsonStorage _pluginSettingJsonStorage;

        public PluginBaseInfo(string pluginRootPath, PluginMetadata pluginMetadata)
        {
            PluginRootPath = pluginRootPath;
            PluginMetadata = pluginMetadata;


            //读取settings
            _pluginSettingJsonStorage = new PluginSettingJsonStorage(pluginMetadata);
            LoadPluginSettings();
        }

        /// <summary>
        ///  plugin's dir
        /// </summary>
        internal string PluginRootPath;


        /// <summary>
        /// Plugin ‘s info
        /// </summary>
        public PluginMetadata PluginMetadata { get; set; }



        /// <summary>
        /// plugin's custom setting
        /// </summary>
        public PluginSettings PluginSettings { get; private set; }


        public string[] PrefixKeywords
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PluginSettings?.PrefixKeyword))
                {
                    return PluginSettings.PrefixKeyword.Split('|');
                }

                if (!string.IsNullOrWhiteSpace(PluginMetadata?.DefalutPrefixKeyword))
                {
                    return PluginMetadata.DefalutPrefixKeyword.Split('|');
                }

                return null;
            }
        }


        ///// <summary>
        ///// plugin 's icon
        ///// </summary>
        //public string PluginIconPath => Path.Combine(PluginRootPath, PluginMetadata.IcoPath);

        /// <summary>
        /// plugin 's entry file
        /// </summary>
        public string PluginEntryPath => Path.Combine(PluginRootPath, PluginMetadata.EntryFileName);

        public void SavePluginSettings()
        {
            _pluginSettingJsonStorage.Save();
        }

        public void LoadPluginSettings()
        {
            PluginSettings = _pluginSettingJsonStorage.Load();
        }

    }

    /// <summary>
    /// store plugin settings in a json file
    /// </summary>
    public class PluginSettingJsonStorage : JsonStrorage<PluginSettings>
    {

        private PluginMetadata _pluginMetadata;

        public PluginSettingJsonStorage(PluginMetadata pluginMetadata)
        {
            _pluginMetadata = pluginMetadata;
            // C# releated, add python releated below
            var directoryPath = Path.Combine(Constant.MyDocumentPath, "Settings", pluginMetadata.PluginId);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            FilePath = Path.Combine(directoryPath, $"Settings{FileSuffix}");
        }

        protected override void LoadDefault()
        {
            if (File.Exists(FilePath))
            {
                BackupOriginFile();
            }

            _data = new PluginSettings
            {
                PrefixKeyword = _pluginMetadata.DefalutPrefixKeyword,
                NeedUpdateIndex = _pluginMetadata.EnableIndex ? (_pluginMetadata.DoIndexWhenEveryStart ? 2 : 1) : -1,
                LastIndexTime = default(DateTime)
            };

            Save();
        }
    }
}
