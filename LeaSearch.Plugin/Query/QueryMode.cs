namespace LeaSearch.Plugin.Query
{
    /// <summary>
    /// 查询模式
    /// </summary>
    public enum QueryMode
    {
        /// <summary>
        /// 命令模式
        /// </summary>
        Command,
        /// <summary>
        /// 插件呼叫模式
        /// </summary>
        PluginCall,
        /// <summary>
        /// 综合建议查询
        /// </summary>
        Suggestion
    }
}