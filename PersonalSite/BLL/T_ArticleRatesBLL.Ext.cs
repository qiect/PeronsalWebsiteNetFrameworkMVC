using DAL;

namespace BLL
{
    public partial class T_ArticleRatesBLL
    {
        /// <summary>
        /// 24小时之内不能重复点赞
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public int Get24HRateCount(string ip, int articleId)
        {
            return new T_ArticleRatesDAL().Get24HRateCount(ip, articleId);
        }
    }
}
