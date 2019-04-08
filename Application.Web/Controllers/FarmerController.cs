﻿using SqlSugar;
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
        public ActionResult GetCurrDayList(int pageNum, int pageSize)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var begDate = $"{datetime} 00:00:00";
                var endDate = $"{datetime} 23:59:59";
                try
                {
                    var dt = db.Ado.GetDataTable($@"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER()OVER(order by op_date) ROWNUM,COUNT(1) OVER() AS Total,* 
                                        FROM(SELECT top 100 percent
                                                 min(tb1.area_ID) as area_ID,
                                                 tb2.Peasant_ID,
                                                 min(tb1.op_date) as op_date,
                                                 min(tb3.Peasant_name) as Peasant_name,
                                                 min(tb3.Peasant_tep) as Peasant_tep,
                                                 min(tb3.Peasant_xiang) as Peasant_xiang,
                                                 min(tb3.Peasant_cun) as Peasant_cun
                                              FROM xunshi tb1 left join area tb2
                                                on tb1.area_ID = tb2.area_ID
                                              left join Peasant tb3
                                                on tb2.Peasant_ID = tb3.Peasant_ID
                                            where tb1.op_date >= @begDate AND tb1.op_date <= @endDate
	                                            group by tb2.Peasant_ID
                                                order by op_date
                                    )A) B where ROWNUM > ({pageNum}  - 1) * {pageSize}", new { begDate, endDate });
                    var jsonData = new { total = dt.Rows[0]["Total"], rows = dt };
                    return ToJsonResult(jsonData);
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
        public ActionResult GetWeekList(int pageNum, int pageSize)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var begDate = DateHelper.GetTimeStartByType("Week", DateTime.Now);
                var endDate = DateHelper.GetTimeEndByType("Week", DateTime.Now);
                try
                {
                    var dt = db.Ado.GetDataTable($@"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER()OVER(order by op_date) ROWNUM,COUNT(1) OVER() AS Total,* 
                                        FROM(SELECT top 100 percent
                                                 min(tb1.area_ID) as area_ID,
                                                 tb2.Peasant_ID,
                                                 min(tb1.op_date) as op_date,
                                                 min(tb3.Peasant_name) as Peasant_name,
                                                 min(tb3.Peasant_tep) as Peasant_tep,
                                                 min(tb3.Peasant_xiang) as Peasant_xiang,
                                                 min(tb3.Peasant_cun) as Peasant_cun
                                              FROM xunshi tb1 left join area tb2
                                                on tb1.area_ID = tb2.area_ID
                                              left join Peasant tb3
                                                on tb2.Peasant_ID = tb3.Peasant_ID
                                            where tb1.op_date >= @begDate AND tb1.op_date <= @endDate
	                                            group by tb2.Peasant_ID
                                                order by op_date
                                    )A) B where ROWNUM > ({pageNum}  - 1) * {pageSize}", new { begDate, endDate });
                    var jsonData = new { total = dt.Rows[0]["Total"], rows = dt };
                    return ToJsonResult(jsonData);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 本月服务农户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMonthList(int pageNum, int pageSize)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var begDate = DateHelper.GetTimeStartByType("Month", DateTime.Now);
                var endDate = DateHelper.GetTimeEndByType("Month", DateTime.Now);
                try
                {
                    var dt = db.Ado.GetDataTable($@"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER()OVER(order by op_date) ROWNUM,COUNT(1) OVER() AS Total,* 
                                        FROM(SELECT top 100 percent
                                                 min(tb1.area_ID) as area_ID,
                                                 tb2.Peasant_ID,
                                                 min(tb1.op_date) as op_date,
                                                 min(tb3.Peasant_name) as Peasant_name,
                                                 min(tb3.Peasant_tep) as Peasant_tep,
                   
                                                 min(tb3.Peasant_xiang) as Peasant_xiang,
                                                 min(tb3.Peasant_cun) as Peasant_cun
                                              FROM xunshi tb1 left join area tb2
                                                on tb1.area_ID = tb2.area_ID
                                              left join Peasant tb3
                                                on tb2.Peasant_ID = tb3.Peasant_ID
                                            where tb1.op_date >= @begDate AND tb1.op_date <= @endDate
	                                            group by tb2.Peasant_ID
                                                order by op_date
                                    )A) B where ROWNUM > ({pageNum}  - 1) * {pageSize}", new { begDate, endDate });
                    var jsonData = new { total = dt.Rows[0]["Total"], rows = dt };
                    return ToJsonResult(jsonData);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 本月未服务农户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMonthNoServerList(int pageNum, int pageSize)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var begDate = DateHelper.GetTimeStartByType("Month", DateTime.Now);
                var endDate = DateHelper.GetTimeEndByType("Month", DateTime.Now);
                try
                {
                    var dt = db.Ado.GetDataTable($@"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER()OVER(order by Peasant_name) ROWNUM,COUNT(1) OVER() AS Total,* 
                                        FROM(select top 100 percent Peasant_name,Peasant_tep,Peasant_ID,Peasant_xiang,Peasant_cun from Peasant where Peasant_ID not in(
                                            SELECT top 100 percent tb2.Peasant_ID
                                                    FROM xunshi tb1 left join area tb2
                                                    on tb1.area_ID = tb2.area_ID
                                                    left join Peasant tb3
                                                    on tb2.Peasant_ID = tb3.Peasant_ID
                                                where tb1.op_date >= '2019-3-1' AND tb1.op_date <= '2019-3-31'
	                                                group by tb2.Peasant_ID
                                    ))A) B where ROWNUM > ({pageNum}  - 1) * {pageSize}", new { begDate, endDate });
                    var jsonData = new { total = dt.Rows[0]["Total"], rows = dt };
                    return ToJsonResult(jsonData);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}