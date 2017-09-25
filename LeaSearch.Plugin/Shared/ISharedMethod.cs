using System.Drawing;
using System.Drawing.Imaging;

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

        ///  <summary>
        /// Show message in UI.
        ///  在主界面弹出消息提示.
        ///  返回国际化的消息提示弹窗
        ///  </summary>
        /// <param name="message"></param>
        bool ShowMessage(string message);

        ///  <summary>
        /// Show message in UI.
        ///  在主界面弹出消息提示.
        ///  返回国际化的消息提示弹窗
        ///  </summary>
        /// <param name="format"></param>
        /// <param name="paramObjects"></param>
        ///  <returns></returns>
        bool ShowMessageWithFormat(string format, params object[] paramObjects);

        /// <summary>
        ///Show message in UI.
        /// 在主界面弹出消息提示.
        /// 返回国际化的消息提示弹窗
        /// </summary>
        /// <param name="i18NKey"></param>
        /// <param name="paramObjects"></param>
        /// <returns></returns>
        bool ShowMessageWithTranslation(string i18NKey, params object[] paramObjects);


        /// <summary>
        /// 记录消息
        /// </summary>
        /// <param name="message"></param>
        void LogInfo(string message);

        /// <summary>
        /// 复制到剪贴板, 并弹出提示框
        /// </summary>
        /// <param name="copyObj"></param>
        void CopyToClipboard(object copyObj);


        /// <summary>
        /// 序列化到json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string SerializeToJson(object obj);

        /// <summary>
        /// 反序列化到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        T DeserializeFromJson<T>(string json);

        /// <summary>
        /// bit map 转 byte数组
        /// </summary>
        /// <param name="bitmap">需要转换的bitmap</param>
        /// <param name="imageFormat">按照该格式转换</param>
        /// <returns></returns>
        byte[] BitmapToBytes(Bitmap bitmap, ImageFormat imageFormat = null);
    }
}