using PersonalSite.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class T_ArticleRatesDAL
    {
        public int Get24HRateCount(string ip, int ArticleId)
        {
            return (int)SqlHelper.ExecuteScalar("select count(*) from T_ArticleRates where IP=@IP and ArticleId=@ArticleId and datediff(hour,CreateDate,getdate())<=24",
                new SqlParameter("@IP", ip),
                new SqlParameter("@ArticleId", ArticleId));
        }
    }
}
