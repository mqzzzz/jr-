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
    public class FarmerController : BaseController
    {
        public ConnectionConfig connStr = new ConnectionConfig() { ConnectionString = ConfigurationManager.ConnectionStrings["BaseDb"].ConnectionString, DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true };
        private string datetime = DateTime.Now.AddDays(-1).ToShortDateString();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 当天服务农户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GeCurrDayList(int pageNum, int pageSize)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var begDate = $"{datetime} 00:00:00";
                var endDate = $"{datetime} 23:59:59";
                try
                {
                    var dt = db.Ado.GetDataTable($@"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER()OVER(order by op_date) ROWNUM,* 
                                        FROM(SELECT
                                                tb1.area_ID,
                                                tb2.Peasant_ID,
                                                tb1.op_date,
                                                tb3.Peasant_name,
                                                tb3.Peasant_tep,
                                                tb2.area_xiang,
                                                tb2.region_Mu
                                              FROM xunshi tb1 left join area tb2
                                                on tb1.area_ID = tb2.area_ID
                                              left join Peasant tb3
                                                on tb2.Peasant_ID = tb3.Peasant_ID
                                            where tb1.op_date >= @begDate AND tb1.op_date <= @endDate
                                    )A) B where ROWNUM > ({pageNum}  - 1) * {pageSize}", new { begDate, endDate });
                    return Success(dt);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 本周服务农户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWeekList()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var begDate = DateHelper.GetTimeStartByType("Week", DateTime.Now);
                var endDate = DateHelper.GetTimeEndByType("Week", DateTime.Now);
                var dt = db.Ado.GetDataTable(@"SELECT 
                                                tb1.area_ID,
                                                tb2.Peasant_ID,
                                                tb1.op_date,
                                                tb3.Peasant_name,
                                                tb3.Peasant_tep,
                                                tb2.area_xiang,
                                                tb2.region_Mu
                                              FROM xunshi tb1 left join area tb2 
                                                on tb1.area_ID=tb2.area_ID
                                              left join Peasant tb3 
                                                on tb2.Peasant_ID=tb3.Peasant_ID 
                                            where tb1.op_date>=@begDate AND tb1.op_date<=@endDate
                                            order by tb1.op_date", new { begDate, endDate });
                return Success(dt);
            }
        }
        /// <summary>
        /// 本月服务农户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMonthList()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var begDate = DateHelper.GetTimeStartByType("Month", DateTime.Now);
                var endDate = DateHelper.GetTimeEndByType("Month", DateTime.Now);
                var dt = db.Ado.GetDataTable(@"SELECT 
                                                tb1.area_ID,
                                                tb2.Peasant_ID,
                                                tb1.op_date,
                                                tb3.Peasant_name,
                                                tb3.Peasant_tep,
                                                tb2.area_xiang,
                                                tb2.region_Mu
                                              FROM xunshi tb1 left join area tb2 
                                                on tb1.area_ID=tb2.area_ID
                                              left join Peasant tb3 
                                                on tb2.Peasant_ID=tb3.Peasant_ID 
                                            where tb1.op_date>=@begDate AND tb1.op_date<=@endDate
                                            order by tb1.op_date", new { begDate, endDate });
                return Success(dt);
            }
        }
        /// <summary>
        /// 本月未服务农户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMonthNoServerList()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"");
                return Success(dt);
            }
        }
    }
}