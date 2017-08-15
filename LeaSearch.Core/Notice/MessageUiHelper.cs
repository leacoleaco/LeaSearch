using System;
using System.Windows;
using LeaSearch.Core.I18N;

namespace LeaSearch.Core.Notice
{
    public class MessageUiHelper
    {

        /// <summary>
        /// 返回国际化的提示信息
        /// </summary>
        /// <param name="i18NKey"></param>
        /// <param name="paramObjects"></param>
        public static void ShowMessage(string i18NKey, params object[] paramObjects)
        {
            var translation = InternationalizationManager.Instance.GetTranslation(i18NKey);
            var s = string.Format(translation, paramObjects);
            MessageBox.Show(s);
        }
    }
}
