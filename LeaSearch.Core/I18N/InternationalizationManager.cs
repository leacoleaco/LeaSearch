namespace LeaSearch.Core.I18N
{
    public class InternationalizationManager
    {
        private static readonly Internationalization _instance = new Internationalization();

        public static Internationalization Instance
        {
            get { return _instance; }
        }
    }
}