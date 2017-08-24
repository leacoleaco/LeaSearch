namespace LeaSearch.Plugin
{
    /// <summary>
    /// 执行命令之后客户端行为及状态等
    /// </summary>
    public class StateAfterCommandInvoke
    {
        /// <summary>
        /// 如果值为true，执行命令完后，不隐藏客户端
        /// </summary>
        public bool ShowProgram { get; set; }
    }
}
