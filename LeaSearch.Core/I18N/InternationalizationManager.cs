using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using LeaSearch.Common.Env;
using LeaSearch.Core.Plugin;
using LeaSearch.Infrastructure.Logger;

namespace LeaSearch.Core.I18N
{
    public class InternationalizationManager
    {
        //public Settings Settings { get; set; }
        private const string Folder = "Languages";
        private const string DefaultFile = "en.xaml";
        private const string Extension = ".xaml";
        private readonly List<string> _languageDirectories = new List<string>();
        private readonly List<ResourceDictionary> _oldResources = new List<ResourceDictionary>();

        private PluginManager _pluginManager;

        public InternationalizationManager(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;

            LoadDefaultLanguage();
            // we don't want to load /Languages/en.xaml twice
            // so add wox language directory after load plugin language files
            AddWoxLanguageDirectory();

            AddPluginLanguageDirectories();
        }



        private void AddWoxLanguageDirectory()
        {
            var directory = Path.Combine(Constant.ProgramDirectory, Folder);
            _languageDirectories.Add(directory);
        }


        private void AddPluginLanguageDirectories()
        {
            foreach (var plugin in _pluginManager.GetPlugins())
            {
                if (!string.IsNullOrWhiteSpace(plugin.PluginRootPath))
                {
                    var pluginThemeDirectory = Path.Combine(plugin.PluginRootPath, Folder);
                    if (Directory.Exists(pluginThemeDirectory))
                    {
                        _languageDirectories.Add(pluginThemeDirectory);
                    }
                    else
                    {
                        Logger.Error($"|Internationalization.AddPluginLanguageDirectories|Can't find plugin path <{pluginThemeDirectory}> for <{plugin.PluginMetadata?.Name}>");
                    }
                }
            }
        }

        private void LoadDefaultLanguage()
        {
            LoadLanguage(AvailableLanguages.English);
            _oldResources.Clear();
        }

        public void ChangeLanguage(string languageCode)
        {
            Language language = GetLanguageByLanguageCode(languageCode);
            ChangeLanguage(language);
        }

        private Language GetLanguageByLanguageCode(string languageCode)
        {
            var lowercase = languageCode.ToLower();
            var language = AvailableLanguages.GetAvailableLanguages().FirstOrDefault(o => o.LanguageCode.ToLower() == lowercase);
            if (language == null)
            {
                Logger.Error($"|Internationalization.GetLanguageByLanguageCode|Language code can't be found <{languageCode}>");
                return AvailableLanguages.English;
            }
            else
            {
                return language;
            }
        }

        public void ChangeLanguage(Language language)
        {

            RemoveOldLanguageFiles();
            if (language != AvailableLanguages.English)
            {
                LoadLanguage(language);
            }
            //UpdatePluginMetadataTranslations();

        }

        private void RemoveOldLanguageFiles()
        {
            var dicts = Application.Current.Resources.MergedDictionaries;
            foreach (var r in _oldResources)
            {
                dicts.Remove(r);
            }
        }

        private void LoadLanguage(Language language)
        {
            var dicts = Application.Current.Resources.MergedDictionaries;
            var filename = $"{language.LanguageCode}{Extension}";
            var files = _languageDirectories
                .Select(d => LanguageFile(d, filename))
                .Where(f => !string.IsNullOrEmpty(f))
                .ToArray();

            if (files.Length > 0)
            {
                foreach (var f in files)
                {
                    var r = new ResourceDictionary
                    {
                        Source = new Uri(f, UriKind.Absolute)
                    };
                    dicts.Add(r);
                    _oldResources.Add(r);
                }
            }
        }

        public List<Language> LoadAvailableLanguages()
        {
            return AvailableLanguages.GetAvailableLanguages();
        }



        /// <summary>
        /// get translation string in language files
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetTranslation(string key)
        {
            var translation = Application.Current.TryFindResource(key);
            if (translation is string)
            {
                return translation.ToString();
            }
            else
            {
                Logger.Error($"|Internationalization.GetTranslation|No Translation for key {key}");
                return $"No Translation for key {key}";
            }
        }

        public string GetTranslation(string i18NKey, object[] paramObjects)
        {
            return String.Format(GetTranslation(i18NKey), paramObjects);
        }

        //private void UpdatePluginMetadataTranslations()
        //{
        //    foreach (var p in PluginManager.GetPluginsForInterface<IPluginI18n>())
        //    {
        //        var pluginI18N = p.Plugin as IPluginI18n;
        //        if (pluginI18N == null) return;
        //        try
        //        {
        //            p.Metadata.Name = pluginI18N.GetTranslatedPluginTitle();
        //            p.Metadata.Description = pluginI18N.GetTranslatedPluginDescription();
        //        }
        //        catch (Exception e)
        //        {
        //            Logger.Exception($"|Internationalization.UpdatePluginMetadataTranslations|Failed for <{p.Metadata.Name}>", e);
        //        }
        //    }
        //}

        public string LanguageFile(string folder, string language)
        {
            if (Directory.Exists(folder))
            {
                string path = Path.Combine(folder, language);
                if (File.Exists(path))
                {
                    return path;
                }
                else
                {
                    Logger.Error($"|Internationalization.LanguageFile|Language path can't be found <{path}>");
                    string english = Path.Combine(folder, DefaultFile);
                    if (File.Exists(english))
                    {
                        return english;
                    }
                    else
                    {
                        Logger.Error($"|Internationalization.LanguageFile|Default English Language path can't be found <{path}>");
                        return string.Empty;
                    }
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}