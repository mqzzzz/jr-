using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Web.Controllers
{
    [LoginCheckFilter(IsCheck = true)]
    public class HomeController : BaseController
    {
        public ConnectionConfig connStr = new ConnectionConfig() { ConnectionString = ConfigurationManager.ConnectionStrings["BaseDb"].ConnectionString, DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true };

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Contact() => View();
        /// <summary>
        /// 左上图片
        /// </summary>
        /// <returns></returns>
        public ActionResult GetXunShi()
        {
            Dictionary<int, dynamic> dic = new Dictionary<int, dynamic>();
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                int i = 0;
                var dt = db.Ado.GetDataTable("select top 4 * from xunshi where Photo1 is not null or Photo2 is not null order by op_date desc");
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        if (!dr["photo1"].IsEmpty())
                        {
                            if (dic.Count < 4)
                            {
                                dic.Add(i, dr["photo1"]);
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (!dr["photo2"].IsEmpty())
                        {
                            if (dic.Count < 4)
                            {
                                dic.Add(i, dr["photo2"]);
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception e) { }


                }
                return Success(dic);
            }

        }
        /// <summary>
        /// 右上农户
        /// </summary>
        /// <returns></returns>
        public ActionResult GetArea()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT TOP 20 
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
                                                on tb2.Peasant_ID=tb3.Peasant_ID order by tb1.op_date desc");
                var dt2 = db.Ado.GetDataTable(@"SELECT 
                                                min(tb1.area_ID) as area_ID,
                                                min(tb1.op_date) as op_date,
                                                min(tb3.Peasant_name) as Peasant_name,
                                                min(tb3.Peasant_tep) as Peasant_tep,
                                                min(tb2.area_xiang) as area_xiang,
                                                sum(tb2.region_Mu) as region_Mu
                                              FROM xunshi tb1 left join area tb2 
                                                on tb1.area_ID=tb2.area_ID
                                              left join Peasant tb3 
                                                on tb2.Peasant_ID=tb3.Peasant_ID group by tb2.Peasant_ID");
                var jsonData = new { dt1 = dt, dt2 = dt2 };
                return Success(jsonData);
            }
        }
        /// <summary>
        /// 左下病虫害
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDIP()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT 
                                                tb1.DIP_Id,
                                                tb1.op_date,
                                                tb2.DIP_name,
                                                tb2.DIP_beizhu,
                                                tb3.area_mubiaojingdu,
                                                tb3.area_mubiaoweidu
                                                FROM xunshi tb1 
                                                left join DIP tb2 on tb1.DIP_Id = tb2.DIP_Id
                                                left join area tb3 on tb1.area_ID = tb3.area_ID 
                                                order by op_date desc");
                return Success(dt);
            }
        }
        /// <summary>
        /// 亩数之和
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRegionCount()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT sum(tb2.region_Mu) as regionCount
                                                    FROM xunshi tb1 left join area tb2
                                                    on tb1.area_ID=tb2.area_ID");
                return Success(dt);
            }
        }
        /// <summary>
        /// 所有采集过的地块
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllArea()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT tb1.*,tb2.Peasant_name FROM area tb1 left join  Peasant tb2 on tb1.Peasant_ID=tb2.Peasant_ID");
                return Success(dt);
            }
        }
        /// <summary>
        /// 病虫害地块
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDipArea()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT * FROM area ");
                return Success(dt);
            }
        }
        public ActionResult GetProject()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT * FROM project order by op_date desc");
                return Success(dt);
            }
        }
        public ActionResult GetAreaByProject(int projectId)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT 
                                                tb1.project_ID,
                                                project_name,
                                                tb2.area_ID,
                                                tb3.area_mubiaojingdu,
                                                tb3.area_mubiaoweidu,
                                                area_zuobiao
                                             FROM project tb1 join projectarea tb2 
                                                on tb1.project_ID=tb2.project_ID 
                                             left join area tb3
                                                on tb2.area_ID=tb3.area_ID where tb1.project_Id=@projectId", new { projectId = projectId });
                return Success(dt);
            }
        }
    }
}