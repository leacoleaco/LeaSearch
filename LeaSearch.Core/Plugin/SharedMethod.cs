using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaSearch.Core.I18N;
using LeaSearch.Core.Notice;
using LeaSearch.Plugin;

namespace LeaSearch.Core.Plugin
{
    public class SharedMethod : ISharedMethod
    {
        /// <summary>
        /// Get translation by key, we could write as string format
        /// 通过 key 翻译字符，可以传入 {0} 等作为参数
        /// </summary>
        /// <param name="i18NKey"></param>
        /// <param name="paramObjects"></param>
        /// <returns></returns>
        public string GetTranslation(string i18NKey, params object[] paramObjects)
        {
            return Ioc.Ioc.Reslove<InternationalizationManager>().GetTranslation(i18NKey, paramObjects);
        }

        /// <summary>
        /// Show message in UI.
        /// 在主界面弹出消息提示.
        /// </summary>
        /// <param name="i18NKey"></param>
        /// <param name="paramObjects"></param>
        /// <returns></returns>
        public bool ShowMessage(string i18NKey, params object[] paramObjects)
        {
            return MessageUiHelper.ShowMessage(i18NKey, paramObjects);
        }
    }
}
