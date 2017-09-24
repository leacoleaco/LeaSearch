using System.Windows.Controls;

namespace LeaSearch.Plugin
{
    /// <summary>
    /// this is the class container info that main program was shared
    /// such as version、i18n、theme、DetailViewControl etc.
    /// 
    /// 本类包含所有与插件共享的方法与数据
    /// </summary>
    public class SharedContext
    {
        public SharedContext(ISharedMethod sharedMethod)
        {
            SharedMethod = sharedMethod;
        }

        /// <summary>
        /// the shared method we can call
        /// </summary>
        public ISharedMethod SharedMethod { get; }


        /// <summary>
        /// the detail view
        /// </summary>
        public ContentControl DetailViewControl { get; internal set; }

    }
}