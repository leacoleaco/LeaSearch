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
        public static bool ShowMessageWithInternation(string i18NKey, params object[] paramObjects)
        {

            var translation = Ioc.Ioc.Reslove<InternationalizationManager>().GetTranslation(i18NKey);
            var s = string.Format(translation, paramObjects);

            return ShowMessage(s);
        }


        /// <summary>
        /// 返回国际化的提示信息
        /// </summary>
        /// <param name="message"></param>
        public static bool ShowMessage(string message)
        {
            MessageBox.Show(message);

            return true;
        }
    }
}
