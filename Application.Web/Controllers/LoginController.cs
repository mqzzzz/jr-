using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Web.Controllers
{
    public class LoginController : BaseController
    {
        public ConnectionConfig connStr = new ConnectionConfig() { ConnectionString = ConfigurationManager.ConnectionStrings["BaseDb"].ConnectionString, DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true };

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CheckLogin(string username, string password)
        {
            password = new Aes().JIAMI(password);
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT * FROM SocketAgriculture_opera
                                                WHERE op_user=@username AND op_pwd=@password", new { username, password });
                if (dt != null && dt.Rows.Count > 0)
                {
                    CookieHelper.SetCookie("user", new Aes().JIAMI(dt.Rows[0]["op_user"].ToString()), DateTime.Now.AddDays(1));
                    return Success("登录成功");
                }
                else
                    return Fail("用户名或密码错误");
            }
        }
    }
}