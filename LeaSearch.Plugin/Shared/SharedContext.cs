using System.Windows.Controls;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin
{
    /// <summary>
    /// this is the class container info that main program was shared
    /// such as version、i18n、theme、DetailViewControl etc.
    /// 
    /// 本类包含所有与插件共享的方法与数据
    ///这些方法都是相通的，主要是一些工具类和 主界面的信息
    /// </summary>
    public class SharedContext
    {
        public SharedContext(ISharedMethod sharedMethod)
        {
            SharedMethod = sharedMethod;
        }

        /// <summary>
        /// current query mode
        /// </summary>
        public QueryMode CurrentQueryMode { get; set; }

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