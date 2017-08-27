namespace LeaSearch.Common.Messages
{
    /// <summary>
    /// 指示主程序隐藏还是显示的消息
    /// </summary>
    public class ShellDisplayMessage
    {
        public Display Display { get; set; }
    }

    public enum Display
    {
        WakeUp, Hide
    }
}
