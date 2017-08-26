using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using LeaSearch.Common.Messages;
using LeaSearch.Core.I18N;

namespace LeaSearch.Core.Notice
{
    public class UiNoticeHelper
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

        public static bool ShowErrorNotice(string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send(new NoticeMessage() { NoticeType = NoticeType.Error, Message = message });
            });
           
            return true;
        }

        public static bool ClearErrorNotice()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send(new NoticeMessage() { NoticeType = NoticeType.Error, Message = null });
            });

            return true;
        }

        public static bool ShowInfoNotice(string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send(new NoticeMessage() { NoticeType = NoticeType.Info, Message = message });
            });

            return true;
        }
    }
}
