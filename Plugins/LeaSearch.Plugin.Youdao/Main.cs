using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using LeaSearch.Plugin.Query;
using LeaSearch.Plugin.Utils;

namespace LeaSearch.Plugin.Youdao
{
    public class TranslateResult
    {
        public int errorCode { get; set; }
        public List<string> translation { get; set; }
        public BasicTranslation basic { get; set; }
        public List<WebTranslation> web { get; set; }
    }

    // 有道词典-基本词典
    public class BasicTranslation
    {
        public string phonetic { get; set; }
        public List<string> explains { get; set; }
    }

    public class WebTranslation
    {
        public string key { get; set; }
        public List<string> value { get; set; }
    }
    public class Main : Plugin
    {
        private const string TranslateUrl = "http://fanyi.youdao.com/openapi.do?keyfrom=WoxLauncher&key=1247918016&type=data&doctype=json&version=1.1&q=";

        public override void InitPlugin(SharedContext sharedContext, IPluginApi pluginApi)
        {
            base.InitPlugin(sharedContext, pluginApi);

            pluginApi.SetIconFromEmbedResource("Images.youdao.ico");
        }

        public override QueryListResult QueryList(QueryParam queryParam)
        {
            var res = new QueryListResult();

            var json = HttpWebRequestUtil.Get(TranslateUrl + queryParam.Keyword);
            if (string.IsNullOrWhiteSpace(json))
            {
                res.ErrorMessage = "请求出错";
            }
            else
            {
                TranslateResult o = SharedContext.SharedMethod.DeserializeFromJson<TranslateResult>(json);
                if (o.errorCode == 0)
                {

                    if (o.translation != null)
                    {
                        var translation = string.Join(", ", o.translation.ToArray());
                        var title = translation;
                        if (o.basic?.phonetic != null)
                        {
                            title += " [" + o.basic.phonetic + "]";
                        }
                        res.AddResultItem(new ResultItem()
                        {
                            Title = title,
                            SubTitle = "翻译结果",
                            SelectedAction = (context =>
                            {
                                SharedContext.SharedMethod.CopyToClipboard(translation);
                                return new StateAfterCommandInvoke();
                            })
                        });
                    }

                    if (o.basic?.explains != null)
                    {
                        var explantion = string.Join(",", o.basic.explains.ToArray());
                        res.AddResultItem(new ResultItem()
                        {
                            Title = explantion,
                            SubTitle = "简明释义",
                            SelectedAction = (context =>
                            {
                                SharedContext.SharedMethod.CopyToClipboard(explantion);
                                return new StateAfterCommandInvoke();
                            })
                        });
                    }

                    if (o.web != null)
                    {
                        foreach (WebTranslation t in o.web)
                        {
                            var translation = string.Join(",", t.value.ToArray());
                            res.AddResultItem(new ResultItem()
                            {
                                Title = translation,
                                SubTitle = "网络释义：" + t.key,
                                SelectedAction = (context =>
                                {
                                    SharedContext.SharedMethod.CopyToClipboard(translation);
                                    return new StateAfterCommandInvoke();
                                })
                            });
                        }
                    }
                }
                else
                {
                    string error = string.Empty;
                    switch (o.errorCode)
                    {
                        case 20:
                            error = "要翻译的文本过长";
                            break;

                        case 30:
                            error = "无法进行有效的翻译";
                            break;

                        case 40:
                            error = "不支持的语言类型";
                            break;

                        case 50:
                            error = "无效的key";
                            break;
                    }

                    res.ErrorMessage = error;
                }
            }
            return res;
        }

    }
}
