namespace LeaSearch.Common.Messages
{
    public class NoticeMessage
    {
        public string Message { get; set; }
        public NoticeType NoticeType { get; set; }
    }

    public enum NoticeType
    {
        Error, Info
    }
}
