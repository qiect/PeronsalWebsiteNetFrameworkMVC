using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helper
{
    /// <summary>
    /// 公共帮助类
    /// </summary>
    public static class CommonHelper
    {

        /// <summary>
        /// 获取配置文件AppSettings结点
        /// </summary>
        /// <param name="name">标签名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public static string SetAppSetting(string name, string val)
        {
            Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
            config.AppSettings.Settings[name].Value = val;
            config.Save();
            return GetAppSetting(name);
        }
        /// <summary>
        /// 获取配置文件AppSettings结点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAppSetting(string name)
        {
            Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
            return config.AppSettings.Settings[name].Value; ;
        }
    }



}
