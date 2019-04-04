using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Web
{
    public class LoginCheckFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 检测是否登录全局过滤器
        /// </summary>
        public bool IsCheck { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (IsCheck)
            {
                //检测用户是否登录
                if (string.IsNullOrEmpty(CookieHelper.GetCookieValue("user")))
                {
                    filterContext.HttpContext.Response.Redirect("/Login/Index");
                }
            }

        }
    }
}