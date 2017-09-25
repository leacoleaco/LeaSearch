namespace LeaSearch.Plugin.Query
{
    /// <summary>
    /// 进行索引的数据
    /// </summary>
    public class DataItem
    {
        public string PluginId { get; set; }

        /// <summary>
        /// 名称。可以索引
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 提示信息，可以索引
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// icon 路径，可以是远程路径
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// icon 数据
        /// </summary>
        public byte[] IconBytes { get; set; }

        /// <summary>
        /// 需要索引的全文
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 附加的额外信息，例如其他数据等
        /// </summary>
        public string Extra { get; set; }
    }
}
