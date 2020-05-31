using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using BLL;
using PersonalSite.Models;
using System;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Model;

namespace PersonalSite.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string code = Request.Form["txtViladateCode"];
                if (string.IsNullOrEmpty(code))
                {
                    ModelState.AddModelError("", "验证码不能为空");
                    return View(model);
                }
                string rightCode = Session["VCode"].ToString();
                if (!code.Equals(rightCode))
                {
                    ModelState.AddModelError("", "验证码输入错误");
                    return View(model);
                }
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "用户名或密码无效.");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostLogin()
        {
            ResultModel<LoginViewModel> result = new ResultModel<LoginViewModel>();
            var form = Request.Form;
            if (form != null)
            {
                string vcode = Session["VCode"].ToString();
                if (vcode.Equals(form["vCode"].ToString()))
                {
                    var user = await UserManager.FindAsync(form["email"], form["pass"]);
                    if (user != null)
                    {
                        await SignInAsync(user, false);
                        result.action = "/Home/Index";
                    }
                    else
                    {
                        result.status = 1;
                        result.msg = "用户名或密码无效";
                    }

                }
                else
                {
                    result.status = 1;
                    result.msg = "验证码错误";
                    result.action = "/Account/Login";
                }

            }

            return Json(result, behavior: JsonRequestBehavior.AllowGet);
        }





        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ValidateCode == Session["VCode"].ToString())
                {
                    var user = new ApplicationUser() { UserName = model.UserName };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        var bll = new T_UserInfosBLL();
                        var userInfo = new T_UserInfos();
                        userInfo.UserId = user.Id;
                        //用QQ来代替邮箱
                        userInfo.Email = model.Email;
                        userInfo.Status = "已激活";
                        userInfo.VCode = Session["VCode"].ToString();
                        var r = bll.InsertDb(userInfo);
                        if (r.Equals(userInfo))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "创建新用户失败");
                        }

                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "验证码不匹配");
                    return View(model);
                }

            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // POST: /Account/PostRegister
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostRegister()
        {
            ResultModel<RegisterViewModel> result = new ResultModel<RegisterViewModel>();
            var form = Request.Form;
            if (form != null)
            {
                string vcode = Session["VCode"].ToString();
                if (vcode.Equals(form["vCode"].ToString()))
                {
                    if (form["pass"].Equals(form["repass"]))
                    {
                        var user = new ApplicationUser() { UserName = form["username"], Email = form["email"], Status = "unactived", VCode = Guid.NewGuid().ToString(), Avatar = "/res/images/avatar/" + new Random().Next(0, 11) + ".jpg" };
                        var res = await UserManager.CreateAsync(user, form["pass"]);
                        if (res.Succeeded)
                        {
                            await SignInAsync(user, isPersistent: false);
                            //注册成功后跳转到注册成功页面，请到邮箱激活
                            //todo:发送邮箱方法有问题，发不过去，要检查
                            if (SendValidateEmail(user.UserName, user.Email, user.VCode))
                            {
                                result.status = 0;
                                //result.msg = "验证邮件发送成功，快去激活您的账号吧！";
                                result.action = "/Account/ValidateUser?un=" + user.UserName;
                            }
                            else
                            {
                                result.status = 1;
                                //result.msg = "注册成功,验证邮箱发送失败！";
                                result.action = "/Account/ValidateUser?un=" + user.UserName;
                            }

                        }
                        else
                        {
                            result.status = 1;
                            result.msg = string.Join(",", res.Errors);
                        }
                    }
                    else
                    {
                        result.status = 1;
                        result.msg = "密码和确认密码不匹配";
                    }

                }
                else
                {
                    result.status = 1;
                    result.msg = "验证码错误";
                    result.action = "/Account/Register";
                }
            }
            return Json(result);
        }


        //
        // GET: /Account/ValidateUser
        [AllowAnonymous]
        public ActionResult ValidateUser(string un, string vc)
        {
            if (!string.IsNullOrEmpty(un))
            {
                if (ValidateVCode(un, vc))
                {
                    ViewBag.Status = "1";
                    ViewBag.Msg = "激活帐户" + un + "成功！";
                }
                else
                {
                    ViewBag.Status = "0";
                    ViewBag.Msg = un + "，快去激活您的账号吧！";
                }
            }
            else
            {
                return View("error");
            }

            return View();
        }

        public bool ValidateVCode(string username, string vcode)
        {
            //todo：用户名不存在、激活码错误、激活码过期（增加一个激活码生成时间字段，只有半个小时之内的才能有效）等几个状态定义为枚举
            var user = UserManager.FindByName(username);
            if (user == null)
            {
                return false;
            }
            //return vcode == userInfo.VCode;
            if (vcode == user.VCode)
            {
                //修改激活状态
                user.Status = "actived";
                UserManager.Update(user);
                return true;
            }
            else
            {
                return false;
            }
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "你的密码已更改。"
                : message == ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
                : message == ManageMessageId.RemoveLoginSuccess ? "已删除外部登录名。"
                : message == ManageMessageId.Error ? "出现错误。"
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 请求重定向到外部登录提供程序
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // 从外部登录提供程序获取有关用户的信息
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }


        //<summary>
        //发送邮箱验证
        //</summary>
        //<param name="userId"></param>
        [HttpPost]
        [AllowAnonymous]
        public bool SendValidateEmail(string userName, string email, string vcode)
        {
            string subject = "验证您的 369社区 账户电子邮件地址";
            string validateUrl = "http://www.hlj3.cn/Account/ValidateUser?un=" + HttpUtility.UrlEncode(userName) + "&vc=" + vcode;
            string content = userName + "，您好！<br/>您申请使用此电子邮件地址访问您的 369社区 账户。点击下方链接验证电子邮件地址。 （如果看不到超链接，请把网址粘贴到您的浏览器打开）：<a href='" + validateUrl + "'>验证电子邮件</a>";
            if (!SendEmailVCode(email, subject, content))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        #region 发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">要发送的邮箱</param>
        /// <param name="mailSubject">邮箱主题</param>
        /// <param name="mailContent">邮箱内容</param>
        /// <returns>返回发送邮箱的结果</returns>
        public bool SendEmailVCode(string mailTo, string mailSubject, string mailContent)
        {
            // 设置发送方的邮件信息,例如使用网易的smtp
            string smtpServer = "smtp.qq.com"; //SMTP服务器
            string mailFrom = "154878690@qq.com"; //登陆用户名
            string userPassword = "mnrxntabcqeocaif";//qq授权码

            // 邮件服务设置
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.Host = smtpServer; //指定SMTP服务器
            smtpClient.Credentials = new System.Net.NetworkCredential(mailFrom, userPassword);//用户名和密码

            // 发送邮件设置       
            MailMessage mailMessage = new MailMessage(mailFrom, mailTo); // 发送人和收件人
            mailMessage.Subject = mailSubject;//主题
            mailMessage.Body = mailContent;//内容
            mailMessage.BodyEncoding = Encoding.UTF8;//正文编码
            mailMessage.IsBodyHtml = true;//设置为HTML格式
            mailMessage.Priority = MailPriority.Low;//优先级

            try
            {
                smtpClient.Send(mailMessage); // 发送邮件
                return true;
            }
            catch (SmtpException ex)
            {
                return false;
            }
        }

        #endregion

        [AllowAnonymous]
        public ActionResult ValidateCode()
        {
            byte[] data = null;
            //string code = RandCode(5);
            int res;
            string code = RandCode(100, out res);
            TempData["code"] = res;
            Session["VCode"] = res;
            //画板
            Bitmap imgCode = new Bitmap(150, 38);
            //画笔
            Graphics gp = Graphics.FromImage(imgCode);
            gp.DrawString(code, new Font("宋体", 15), Brushes.White, new PointF(15, 8));
            Random random = new Random();
            //画噪线
            for (int i = 0; i < 5; i++)
            {
                gp.DrawLine(new Pen(RandColor()), random.Next(imgCode.Width), random.Next(imgCode.Height), random.Next(imgCode.Width), random.Next(imgCode.Height));
            }
            //画噪点
            for (int i = 0; i < 50; i++)
            {
                //指定像素的颜色来画噪点
                imgCode.SetPixel(random.Next(imgCode.Width), random.Next(imgCode.Height), RandColor());
            }

            gp.Dispose();
            //创建内存流
            MemoryStream ms = new MemoryStream();
            imgCode.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            data = ms.GetBuffer();
            return File(data, "image/jpeg");
        }

        private Color RandColor()
        {
            Random random = new Random();
            int red = random.Next(10, 240);
            int green = random.Next(10, 240);
            int blue = random.Next(10, 240);
            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// 随机生成指定长度的验证码
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        private string RandCode(int len)
        {
            //string words = "1234567890" + "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + "abcdefghijklmnopqrstuvwxyz";
            //StringBuilder sb = new StringBuilder();
            //Random random = new Random();
            //for (int i = 0; i < len; i++)
            //{
            //    int index = random.Next(0, words.Length);
            //    char c = words[index];
            //    if (sb.ToString().Contains(c))
            //    {
            //        i--;
            //        continue;
            //    }
            //    sb.Append(words[index] + "");
            //}
            //return sb.ToString();
            string str = Guid.NewGuid().ToString();
            return str.Substring(str.Length - len);
        }

        private string RandCode(int max, out int result)
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            int i = random.Next(max), j = random.Next(max);
            sb.Append("=  ");
            sb.Append(i + "  +  " + j);
            result = i + j;
            return sb.ToString();
        }

        public ActionResult Index()
        {
            return View();
        }

        #region 帮助程序
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}