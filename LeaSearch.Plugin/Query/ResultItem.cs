﻿using System;
using System.Windows.Media;

namespace LeaSearch.Plugin
{

    /// <summary>
    /// return list item info
    /// </summary>
    public class ResultItem
    {
        /// <summary>
        /// 标识得到这条结果的插件
        /// 返回结果时候无需设置
        /// </summary>
        public string PluginId { get; set; }
        /// <summary>
        /// 标识查询参数，返回结果时候无需设置
        /// </summary>
        public QueryParam QueryParam { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string SubTitle { get; set; }

        /// <summary>
        /// 列表图标的路径
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// 附加信息，可以用于预览中获取等
        /// </summary>
        public object ExtraInfo { get; set; }

        /// <summary>
        /// choose after action
        /// return true to hide leasearch after selected 
        /// </summary>
        public Func<SharedContext, StateAfterCommandInvoke> SelectedAction { get; set; }


    }
}