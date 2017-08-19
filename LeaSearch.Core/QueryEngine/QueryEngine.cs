using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeaSearch.Core.Plugin;
using LeaSearch.Plugin;

namespace LeaSearch.Core.QueryEngine
{
    public class QueryEngine
    {
        /// <summary>
        /// this is to judge if this query is a command
        /// </summary>
        private const char QueryCommandChar = ':';

        /// <summary>
        /// this is the split word that use to indicate action word   
        /// </summary>
        private const char QueryActionSplitChar = ' ';

        private const char NoneQueryPrefix = '*';

        private CancellationTokenSource _updateSource;
        private CancellationToken _updateToken;
        private bool _queryHasReturn;

        private PluginManager _pluginManager;

        public QueryEngine(PluginManager pluginManager)
        {
            this._pluginManager = pluginManager;
        }

        public void Init()
        {

        }

        public void Query(string queryText)
        {
            if (string.IsNullOrWhiteSpace(queryText)) return;

            //we need to trim start space
            queryText = queryText.TrimStart();

            _updateSource?.Cancel();
            _updateSource = new CancellationTokenSource();
            _updateToken = _updateSource.Token;

            _queryHasReturn = false;


            var queryParam = new QueryParam
            {
                OriginQueryStr = queryText,
                QueryType = QueryType.List
            };


            if (!queryText.Contains(QueryActionSplitChar))
            {
                // do the normal search
                queryParam.Keyword = queryText;
                queryParam.PrefixKeyword = "*";
            }
            else
            {
                //do the prefix search
                var actionCharIndex = queryText.IndexOf(QueryActionSplitChar);
                string part1 = queryText.Substring(0, actionCharIndex);
                string part2 = queryText.Substring(actionCharIndex + 1);
                queryParam.PrefixKeyword = part1;
                queryParam.Keyword = part2;

            }


            Task.Delay(200, _updateToken).ContinueWith(_ =>
            {

            }, _updateToken);

            var suitableQueryPlugins = SearchSuitablePlugins(queryParam);
            OnGetSuitablePlugins(suitableQueryPlugins);
            if (suitableQueryPlugins == null || !suitableQueryPlugins.Any())
            {
                return;
            }

            Task.Run(() =>
            {
                //Dictionary<Plugin.Plugin, QueryListResult> results = new Dictionary<Plugin.Plugin, QueryListResult>();
                //Parallel.ForEach(suitableQueryPlugins, plugin =>
                //{
                //    var queryListResult = plugin.PluginInstance.Query(queryParam);
                //    results.Add(plugin, queryListResult);
                //});

                var queryListResult = suitableQueryPlugins[0].PluginInstance.Query(queryParam);
                //return the result
                OnGetResult(queryListResult);

            }, _updateToken);
        }


        private Plugin.Plugin[] SearchSuitablePlugins(QueryParam queryParam)
        {
            var result = new List<Plugin.Plugin>();
            _pluginManager.GetPlugins()?.ForEach(p =>
            {
                if (p.IsDisabled) return;

                var prefixKeywords = p.PrefixKeywords;
                if (prefixKeywords == null || prefixKeywords.Length <= 0) return;

                if (prefixKeywords.Contains(queryParam.PrefixKeyword))
                {
                    //command mode, display in first choice
                    result.Insert(0, p);
                }
                else if (p.PluginInstance.SuitableForThisQuery(queryParam))
                {
                    //other plugin is add to second choice if suitable
                    result.Add(p);
                }
            });
            return result.ToArray();
        }


        #region Event

        public event Action<QueryListResult> GetResult;
        public event Action<Plugin.Plugin[]> GetSuitablePlugins;

        #endregion

        protected virtual void OnGetResult(QueryListResult result)
        {
            GetResult?.Invoke(result);
        }

        protected virtual void OnGetSuitablePlugins(Plugin.Plugin[] suitablePlugins)
        {
            GetSuitablePlugins?.Invoke(suitablePlugins);
        }
    }
}
