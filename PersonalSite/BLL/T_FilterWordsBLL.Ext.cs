using DAL;
using Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BLL
{
    public partial class T_FilterWordsBLL
    {
        //重用
        private static string bannedExprKey = typeof(T_FilterWordsBLL) + "BannedExpr";
        private static string modExprKey = typeof(T_FilterWordsBLL) + "ModExpr";

        public T_FilterWords GetByWord(string word)
        {
            return GetList(new { WordPattern = word }).FirstOrDefault();
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearCache()
        {
            //string bannedExprKey = typeof(RP_FilterWordBLL) + "BannedExpr";
            //string modExprKey = typeof(RP_FilterWordBLL) + "ModExpr";

            HttpRuntime.Cache.Remove(bannedExprKey);
            HttpRuntime.Cache.Remove(modExprKey);
        }


        /// <summary>
        /// 取得禁用词的正则表达式
        /// 由方法去处理缓存的问题
        /// </summary>
        /// <returns>返回禁用词正则表达式</returns>
        private string GetBannedExpr()
        {
            //因为缓存是整个网站唯一的实例，所以要保证key的唯一性
            //我的习惯就是在Cache的名字前加一个类名，几乎不会重复
            //项目组规定，缓存的名字都是类型全名+缓存项的名字
            string bannedExpr = (string)HttpRuntime.Cache[bannedExprKey];
            if (bannedExpr != null)//如果缓存中存在，则直接返回
            {
                return bannedExpr;
            }

            var banned = (from word in GetBanned()
                          select word.WordPattern).ToArray();
            bannedExpr = string.Join("|", banned);

            bannedExpr = bannedExpr.Replace(@".", @"\.").Replace("{2}", ".{0,2}").Replace(@"\", @"\\");
            //如果不存在则去数据库取，然后放入缓存
            HttpRuntime.Cache.Insert(bannedExprKey, bannedExpr);
            return bannedExpr;
        }

        private string GetModExpr()
        {
            string modExpr = (string)HttpRuntime.Cache[modExprKey];
            if (modExpr != null)//如果缓存中存在，则直接返回
            {
                return modExpr;
            }
            var mod = (from word in GetMod()
                       select word.WordPattern).ToArray();
            modExpr = string.Join("|", mod);

            modExpr = modExpr.Replace(@".", @"\.").Replace("{2}", ".{0,2}").Replace(@"\", @"\\");
            //如果不存在则去数据库取，然后放入缓存
            HttpRuntime.Cache.Insert(modExprKey, modExpr);
            return modExpr;
        }

        private IEnumerable<T_FilterWords> GetReplaceWords()
        {
            string replaceKey = typeof(T_FilterWordsBLL) + "Replace";
            IEnumerable<T_FilterWords> data = (IEnumerable<T_FilterWords>)HttpRuntime.Cache[replaceKey];
            if (data != null)//如果缓存中存在，则直接返回
            {
                return data;
            }
            data = GetReplace();
            //如果不存在则去数据库取，然后放入缓存
            HttpRuntime.Cache.Insert(replaceKey, data);
            return data;
        }

        public FilterResult FilterMsg(string msg, out string replacemsg)
        {
            string bannedExpr = GetBannedExpr();

            //todo：避免每次进行过滤词判断的时候都进行数据库查询。缓存，把bannedExpr缓存起来。


            string modExpr = GetModExpr();


            //先判断禁用词！有禁用词就不让发，无从谈起审核词
            //Regex.Replace(
            foreach (var word in GetReplaceWords())
            {
                msg = msg.Replace(word.WordPattern, word.ReplaceWord);
            }
            replacemsg = msg;

            if (Regex.IsMatch(replacemsg, bannedExpr))
            {
                return FilterResult.Banned;
            }
            else if (Regex.IsMatch(replacemsg, modExpr))
            {
                return FilterResult.Mod;
            }


            return FilterResult.OK;
        }

        /// <summary>
        /// 从流中导入过滤词
        /// </summary>
        /// <param name="stream"></param>
        public void Import(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.Default))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] strs = line.Split('=');
                    string word = strs[0];
                    string replaceWord = strs[1];
                    //判断是否已经存在。存在则更新，不存在直接插入
                    var filterWord = GetByWord(word);
                    if (filterWord == null)
                    {
                        filterWord = new T_FilterWords();
                        filterWord.ReplaceWord = replaceWord;
                        filterWord.WordPattern = word;
                        InsertDb(filterWord);
                    }
                    else
                    {
                        //如果存在则更新
                        filterWord.ReplaceWord = replaceWord;
                        filterWord.WordPattern = word;
                        InsertDb(filterWord);
                    }
                }
            }
        }

        /// <summary>
        /// 获取BANNED列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T_FilterWords> GetBanned()
        {
            return GetList(new { ReplaceWord = "{BANNED}" });
        }
        /// <summary>
        /// 获取MOD列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T_FilterWords> GetMod()
        {
            return GetList(new { ReplaceWord = "{MOD}" });
        }
        /// <summary>
        /// 获取替换词列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T_FilterWords> GetReplace()
        {
            return GetList("where ReplaceWord<>'{MOD}' and ReplaceWord<>'{BANNED}'",null);
        }
    }
}
