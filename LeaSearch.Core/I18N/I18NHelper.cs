namespace LeaSearch.Core.I18N
{
    public static class I18NHelper
    {
        public static string GetTranslation(this string i18NKey, params object[] paramObjects)
        {
            var internationalizationManager = Ioc.Ioc.Reslove<InternationalizationManager>();
            return internationalizationManager?.GetTranslation(i18NKey, paramObjects);
        }
    }
}
