namespace LeaSearch.Plugin
{
    public interface ISharedMethod
    {
        /// <summary>
        /// 返回国际化的提示信息
        /// </summary>
        /// <param name="i18NKey"></param>
        /// <param name="paramObjects"></param>
        string GetTranslation(string i18NKey, params object[] paramObjects);

        /// <summary>
        /// 返回国际化的消息提示弹窗
        /// </summary>
        /// <param name="i18NKey"></param>
        /// <param name="paramObjects"></param>
        /// <returns></returns>
        bool ShowMessage(string i18NKey, params object[] paramObjects);
    }
}