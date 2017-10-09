namespace LeaSearch.Plugin
{
    public class PluginCalledArg
    {
        /// <summary>
        /// 在激活插件后，展示的提示信息
        /// </summary>
        public string InfoMessage { get; set; }

        /// <summary>
        /// 插件使用的查询方式
        /// </summary>
        public ResultMode ResultMode { get; set; }
    }

    public enum ResultMode
    {
        List,
        Detail
    }
}
