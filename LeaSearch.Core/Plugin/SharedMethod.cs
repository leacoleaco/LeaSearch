using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using LeaSearch.Core.I18N;
using LeaSearch.Core.Notice;
using LeaSearch.Infrastructure.Logger;
using LeaSearch.Plugin;
using Newtonsoft.Json;

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

        public bool ShowMessage(string message)
        {
            return UiNoticeHelper.ShowMessage(message);
        }

        public bool ShowMessageWithFormat(string format, params object[] paramObjects)
        {
            return UiNoticeHelper.ShowMessage(string.Format(format, paramObjects));
        }

        public bool ShowMessageWithTranslation(string i18NKey, params object[] paramObjects)
        {
            return UiNoticeHelper.ShowMessageWithInternation(i18NKey, paramObjects);
        }


        public void LogInfo(string message)
        {
            Logger.Info(message);
        }

        public void CopyToClipboard(object copyObj)
        {
            if (copyObj != null)
            {
                try
                {
                    Clipboard.SetDataObject(copyObj);
                    ShowMessageWithFormat(@"message_copyToClipboard", copyObj.ToString());
                }
                catch (Exception e)
                {
                    ShowMessageWithTranslation(@"message_copyToClipboardFailure", copyObj.ToString(), e.Message);
                    throw;
                }

            }
        }

        public string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T DeserializeFromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public byte[] BitmapToBytes(Bitmap bitmap, ImageFormat imageFormat = null)
        {
            if (imageFormat == null)
            {
                imageFormat = ImageFormat.Jpeg;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, imageFormat);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }
    }
}
