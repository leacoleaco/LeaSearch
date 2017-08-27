using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LeaSearch.Core.Views;
using LeaSearch.Plugin;

namespace LeaSearch.Core.Command
{
    /// <summary>
    /// 本类主要管理 工程中的所有可用命令
    /// </summary>
    public class LeaSearchCommandManager
    {
        private readonly Dictionary<string, CommandInfo> _actions = new Dictionary<string, CommandInfo>();

        public LeaSearchCommandManager()
        {
            RegistAction("htmleditor", new CommandInfo()
            {
                Name = "HTMl Editor",
                Introduce = "edit html helper",
                Action = (queryParam) =>
                {
                    var view = new HtmlEditorView();
                    view.Show();
                },
            });


            RegistAction("shutdown", new CommandInfo()
            {
                Name = "Shutdown Computer",
                Introduce = "Shutdown this computer",
                Action = (queryParam) =>
                {
                    var view = new HtmlEditorView();
                    view.Show();
                },
            });

            RegistAction("exit", new CommandInfo()
            {
                Name = "Exit Program",
                Introduce = "exit LeaSearch",
                Action = (queryParam) =>
                {
                   Application.Current.Shutdown();
                },
            });


        }

        private void RegistAction(string keyword, CommandInfo info)
        {
            _actions.Add(keyword, info);
        }


        public void InvokeCommand(QueryParam queryParam)
        {
            var action = _actions[queryParam.Keyword];

            action?.Action?.Invoke(queryParam);
        }

        public string[] GetKeywords()
        {
            return _actions.Keys.ToArray();
        }

        public List<CommandInfo> GetCommandInfo(string[] keywords)
        {
            var result = new List<CommandInfo>();
            foreach (var keyword in keywords)
            {
                var commandInfo = _actions[keyword];
                if (commandInfo != null)
                {
                    result.Add(commandInfo);
                }
            }
            return result;
        }
    }

    public class CommandInfo
    {
        public CommandInfo()
        {

        }

        public string Name { get; internal set; }
        public string Introduce { get; internal set; }
        public Action<QueryParam> Action { get; internal set; }
    }
}
