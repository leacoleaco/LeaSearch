using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace LeaSearch.Plugin.Utils
{
    public class HttpWebRequestUtil
    {
        public static string Get(string url)
        {
            Uri uri = new Uri(url);//创建uri对象，指定要请求到的地址  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);//使用HttpWebRequest类的Create方法创建一个请求到uri的对象。 
            request.Method = WebRequestMethods.Http.Get;//指定请求的方式为Get方式 

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();//获取该请求所响应回来的资源，并强转为HttpWebResponse响应对象 
            StreamReader reader = new StreamReader(response.GetResponseStream());//获取该响应对象的可读流 

            string responseStr = reader.ReadToEnd(); //将流文本读取完成并赋值给str 
            response.Close(); //关闭响应 

            return responseStr;
        }

        public static string Post(string url, KeyValuePair<string, string>[] datas)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;//指定请求的方式为Post方式 
            var data = "";
            foreach (var d in datas)
            {
                data += $"{d.Key}={d.Value}&";
            }

            request.ContentLength = data.Length; //指定要请求参数的长度 
            request.ContentType = "application/x-www-form-urlencoded"; //指定请求的内容类型 

            StreamWriter writer = new StreamWriter(request.GetRequestStream()); //用请求对象创建请求的写入流
            writer.Write(data); //将请求的参数列表写入到请求对象中 
            writer.Close(); //关闭写入流。 

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            string responseStr = reader.ReadToEnd();
            response.Close();
            return responseStr;
        }
    }
}
