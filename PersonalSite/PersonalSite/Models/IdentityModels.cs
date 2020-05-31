using Microsoft.AspNet.Identity.EntityFramework;

namespace PersonalSite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get;
            set;
        }
        /// <summary>
        /// 状态：是否激活
        /// </summary>
        public string Status
        {
            get;
            set;
        }
        /// <summary>
        /// 激活码
        /// </summary>
        public string VCode
        {
            get;
            set;
        }
        /// <summary>
        /// 信用积分
        /// </summary>
        public int? Credit
        {
            get;
            set;
        }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar
        {
            get;
            set;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}