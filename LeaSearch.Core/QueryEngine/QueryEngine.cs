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

        public void Query(string queryText, Action<Dictionary<Plugin.Plugin, QueryListResult>> GetResultAction)
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

            var suitableQueryPlugins = GetSutablePlugins(queryParam);
            Task.Run(() =>
            {
                Dictionary<Plugin.Plugin, QueryListResult> results = new Dictionary<Plugin.Plugin, QueryListResult>();
                Parallel.ForEach(suitableQueryPlugins, plugin =>
                {
                    var queryListResult = plugin.PluginInstance.Query(queryParam);
                    results.Add(plugin, queryListResult);
                });

                //return the result
                GetResultAction?.Invoke(results);

            }, _updateToken);
        }


        private IEnumerable<Plugin.Plugin> GetSutablePlugins(QueryParam queryParam)
        {
            return _pluginManager.GetPlugins()?.Where(p =>
            {
                if (p.IsDisabled)
                {
                    return false;
                }

                var pk = p.PrefixKeyword;
                if (!string.IsNullOrWhiteSpace(pk))
                {
                    return pk == queryParam.PrefixKeyword;
                }
                return false;
            }).ToList();
        }



    }
}
