using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Web.Controllers
{
    [LoginCheckFilter(IsCheck = true)]
    public class ChartController : BaseController
    {
        public ConnectionConfig connStr = new ConnectionConfig() { ConnectionString = ConfigurationManager.ConnectionStrings["BaseDb"].ConnectionString, DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true };
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 病虫害图标
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDipChart()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"select tb1.*,tb2.DIP_name from (SELECT  count(xunshi_ID) as total
                                              ,min(DIP_Id) as DIP_Id
                                          FROM xunshi where DIP_Id<>6 group by DIP_Id) as tb1 join DIP tb2
                                          on tb1.DIP_Id=tb2.DIP_Id");
                return Success(dt);
            }
        }
        public ActionResult GetSowChart()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"select tb1.*,tb2.Seed_name from (SELECT  count(Sow_ID) as total
                                              ,min(Seed_ID) as Seed_ID
                                          FROM [AgricultureData].[dbo].Sow  group by Seed_ID) as tb1 join Seed tb2
                                          on tb1.Seed_ID=tb2.Seed_ID");
                return Success(dt);
            }
        }
        public ActionResult GetDaYaoChart()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"  select tb1.*,tb2.Pesticides_name from (SELECT  count([sprayagricultural_GUID]) as total
                                                  ,min(Pesticides_ID) as Pesticides_ID
                                              FROM sprayagricultural  group by Pesticides_ID) as tb1 join Pesticides tb2
                                              on tb1.Pesticides_ID=tb2.Pesticides_ID");
                return Success(dt);
            }
        }
        public ActionResult GetZhengDiChart()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"  select tb1.*,tb2.Landrectification_name from (SELECT  count(zhengdi_ID) as total
                                                  ,min(Landrectification_ID) as Landrectification_ID
                                              FROM zhengdi  group by Landrectification_ID) as tb1 join Landrectification tb2
                                              on tb1.Landrectification_ID=tb2.Landrectification_ID");
                return Success(dt);
            }
        }
    }
}