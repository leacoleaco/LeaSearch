namespace LeaSearch.Common.Messages
{
    public class FocusMessage
    {
        public FocusTarget FocusTarget { get; set; }
    }

    public enum FocusTarget
    {
        QueryTextBox,ResultList
    }
}
