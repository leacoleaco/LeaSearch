﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LeaSearch.Common.Env
{
    public static class Constant
    {
        public const string LeaSearch = "LeaSearch";
        public const string Plugins = "Plugins";

        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        /// <summary>
        /// get the program directory
        /// </summary>
        public static readonly string ProgramDirectory = Directory.GetParent(Assembly.Location).ToString();
        public static readonly string ExecutablePath = Path.Combine(ProgramDirectory, LeaSearch + ".exe");

        /// <summary>
        /// the directory that load data
        /// </summary>
        //public static readonly string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LeaSearch);
        public static readonly string DataDirectory = "Data";

        /// <summary>
        /// the plugin's dir
        /// </summary>
        public static readonly string PluginsDirectory = Path.Combine(ProgramDirectory, Plugins);

        public static readonly string PreinstalledDirectory = Path.Combine(ProgramDirectory, Plugins);


        public const string Repository = "https://github.com/Wox-launcher/Wox";
        public const string Issue = "https://github.com/Wox-launcher/Wox/issues/new";
        public static readonly string Version = FileVersionInfo.GetVersionInfo(Assembly.Location).ProductVersion;

        public static readonly string DefaultIcon = Path.Combine(ProgramDirectory, "Images", "app.png");
        public static readonly string ErrorIcon = Path.Combine(ProgramDirectory, "Images", "app_error.png");

        public static string PythonPath;
        public static string EverythingSDKPath;
    }
}
