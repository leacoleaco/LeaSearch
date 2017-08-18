using System.IO;
using LeaSearch.Common.Env;

namespace LeaSearch.Infrastructure.Storage
{
    /// <summary>
    /// store plugin settings in a json file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PluginSettingJsonStorage<T> : JsonStrorage<T> where T : new()
    {
        public const string DirectoryName = "Settings\\PluginSettings";

        public PluginSettingJsonStorage()
        {
            // C# releated, add python releated below
            var dataType = typeof(T);
            var assemblyName = typeof(T).Assembly.GetName().Name;
            var directoryPath = Path.Combine(Constant.ProgramDirectory, DirectoryName, assemblyName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            FilePath = Path.Combine(directoryPath, $"{dataType.Name}{FileSuffix}");
        }

    }
}
