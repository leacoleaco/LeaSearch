﻿using System.Collections.Generic;

namespace LeaSearch.Plugin
{
    public interface IPlugin
    {
        /// <summary>
        /// let plugin that may get the share info
        /// </summary>
        /// <param name="sharedContext"></param>
        void InitPlugin(SharedContext sharedContext);


        /// <summary>
        /// check the queryParam to judge should we do query method
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        bool SuitableForThisQuery(QueryParam queryParam);

        /// <summary>
        /// query result for list 
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        QueryListResult Query(QueryParam queryParam);

        /// <summary>
        /// query a detail info
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        QueryDetailResult QueryDetail(QueryParam queryParam);
    }
}