using Microsoft.AspNet.SignalR;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Application.Web.Controllers
{
    [LoginCheckFilter(IsCheck = true)]
    public class HomeController : BaseController
    {
        public ConnectionConfig connStr = new ConnectionConfig() { ConnectionString = ConfigurationManager.ConnectionStrings["BaseDb"].ConnectionString, DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true };
        private string datetime = DateTime.Now.AddDays(-1).ToShortDateString();
        public ActionResult Index()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        using (SqlSugarClient db = new SqlSugarClient(connStr))
                        {
                            var dt = db.Ado.GetDataTable(@" select 
                                                            a.[nongji_ID],
                                                            shebei_date,
                                                            shebei_jingdu,
                                                            shebei_weidu,
                                                            shebei_sudu,
                                                            nongji_name
                                                            from [sbpfallT] as a 
                                                            left join nongji nj 
                                                            on a.nongji_ID=nj.nongji_ID
                                                            where shebei_date = (select max(b.shebei_date) 
                                                            from [sbpfallT] as b  
                                                            where a.[nongji_ID] = b.[nongji_ID] ) 
                                                            group by  a.[nongji_ID],shebei_date,shebei_jingdu,shebei_weidu,shebei_sudu,nongji_name 
                                                            order by shebei_date desc");
                            await GlobalHost.ConnectionManager.GetHubContext<MyHub>().Clients.All.sendMessage(dt);
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    await Task.Delay(2000);
                }
            });
            return View();
        }

        public ActionResult CollectHistory() => View();
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
                var dt = db.Ado.GetDataTable("select top 4 * from xunshi where Photo1 is not null or Photo2 is not null  order by op_date desc");
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
                var begDate = $"{datetime} 00:00:00";
                var endDate = $"{datetime} 23:59:59";
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
                                                on tb2.Peasant_ID=tb3.Peasant_ID 
                                             where tb1.op_date >= @begDate AND tb1.op_date <= @endDate
                                            order by tb2.region_Mu desc", new { begDate, endDate });
                var dt2 = db.Ado.GetDataTable(@"SELECT top 4
                                                min(tb1.area_ID) as area_ID,
                                                min(tb1.op_date) as op_date,
                                                min(tb3.Peasant_name) as Peasant_name,
                                                min(tb3.Peasant_tep) as Peasant_tep,
                                                min(tb2.area_xiang) as area_xiang,
                                                sum(tb2.region_Mu) as region_Mu
                                                FROM xunshi tb1 left join area tb2 
                                                on tb1.area_ID=tb2.area_ID
                                                left join Peasant tb3 
                                                on tb2.Peasant_ID=tb3.Peasant_ID
                                                where tb1.op_date >= @begDate AND tb1.op_date <= @endDate
                                                 group by tb2.Peasant_ID order by region_Mu desc", new { begDate, endDate });
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
 where tb1.DIP_Id<>6 and CONVERT(varchar(100), tb1.op_date, 102)>=CONVERT(varchar(100), DateAdd(dd,-7,getdate()), 102)
                                                order by tb1.op_date desc");
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
                var begDate = $"{datetime} 00:00:00";
                var endDate = $"{datetime} 23:59:59";
                var dt = db.Ado.GetDataTable(@"SELECT sum(tb2.region_Mu) as regionCount
                                                    FROM xunshi tb1 left join area tb2
                                                    on tb1.area_ID=tb2.area_ID
													where tb1.op_date >= @begDate AND tb1.op_date <= @endDate", new { begDate, endDate });
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
                var dt = db.Ado.GetDataTable(@" SELECT 
                                                TB1.OP_DATE,
                                                TB1.AREA_ID,
                                                TB1.DIP_ID,
                                                area_zuobiao,
                                                area_mubiaojingdu,
                                                area_mubiaoweidu,
                                                area_xiang,
                                                area_cun,
                                                DIP_name,
                                                DIP_yanse 
                                                FROM  
                                                (SELECT TOP 1000 MIN([xunshi_ID]) AS XUNSHI_ID
                                                  ,MIN([op_date]) AS OP_DATE
                                                  ,MIN([area_ID]) AS AREA_ID
                                                  ,MIN([DIP_Id]) AS DIP_ID
                                              FROM xunshi where DIP_Id<>6 
                                              GROUP BY area_ID,DIP_Id   ORDER BY OP_DATE DESC) AS TB1 
                                                JOIN area TB2 ON TB1.AREA_ID=TB2.area_ID
                                                JOIN DIP TB3 ON TB1.DIP_ID=TB3.DIP_Id");
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
        public ActionResult GetNongJiList()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"select 
                                                tb2.*,
                                                tb3.nongjitype_name 
                                                from(SELECT nongji_ID FROM sbpfallT group by nongji_ID) as tb1
                                                left join nongji tb2 on tb1.nongji_ID = tb2.nongji_ID
                                                left join nongjitype tb3 on tb2.nongjitype_ID = tb3.nongjitype_ID");
                return Success(dt);
            }

        }
        /// <summary>
        /// 根据农机ID和时间范围获取农机轨迹
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNongjiTrail(int nongjiId, DateTime datetime)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var begdate = datetime.ToShortDateString() + " 00:00:00";
                var enddate = datetime.ToShortDateString() + " 23:59:59";
                var dt = db.Ado.GetDataTable(@"select * from sbpfallT where nongji_ID=@nongjiId and shebei_date>=@begdate and shebei_date<=@enddate",
                    new { nongjiId, begdate, enddate });
                return Success(dt);
            }
        }
        public ActionResult GetDipByMonth()
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT 
                                                COUNT(TB1.DIP_Id) as total,
                                                tb2.DIP_name
                                                FROM xunshi tb1 
                                                 join DIP tb2 on tb1.DIP_Id = tb2.DIP_Id
                                                left join area tb3 on tb1.area_ID = tb3.area_ID 
                                             where tb1.DIP_Id<>6 and tb1.op_date>=CONVERT(varchar(100), DateAdd(MM,-1,getdate()), 102)
                                             GROUP BY TB1.DIP_Id,DIP_name");
                return Success(dt);
            }

        }
        /// <summary>
        /// 获取巡视历史列表（根据区域ID）
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public ActionResult GetXunShiByAreaId(int areaId)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT op_date,xunshi_beizhu,Photo1,Photo2,tb1.DIP_Id,tb2.DIP_name FROM xunshi tb1 
                                                join DIP tb2 on tb1.DIP_Id=tb2.DIP_Id 
                                                 WHERE area_ID=@areaId order by op_date desc", new { areaId });
                return Success(dt);
            }
        }
        /// <summary>
        /// 获取亩产历史列表（根据区域ID）
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public ActionResult GetMuchanByAreaId(int areaId)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT muchan_count,op_date,muchan_beizhu FROM muchan WHERE area_ID=@areaId order by op_date desc", new { areaId });
                return Success(dt);
            }
        }
        /// <summary>
        /// 获取管理历史列表（根据区域ID）
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public ActionResult GetManageByAreaId(int areaId)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT tb1.op_date,manage_beizhu,managetype_name
                                              FROM manage tb1 left join managetype tb2
                                              on tb1.managetype_ID=tb2.managetype_ID
                                              where area_ID=@areaId order by op_date desc", new { areaId });
                return Success(dt);
            }
        }
        /// <summary>
        /// 获取整地历史列表（根据区域ID）
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public ActionResult GetZhengDiByAreaId(int areaId)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT op_date,Landrectification_name,zhengdi_beizhu 
                                                FROM zhengdi tb1
                                                left join Landrectification tb2
                                                on tb1.Landrectification_ID=tb2.Landrectification_ID
                                                 WHERE area_ID=@areaId order by op_date desc", new { areaId });
                return Success(dt);
            }
        }
        /// <summary>
        /// 获取施肥历史列表（根据区域ID）
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public ActionResult GetShiFeiByAreaId(int areaId)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"select op_date from  Applyfertilizer where area_ID=@areaId order by op_date desc", new { areaId });
                return Success(dt);
            }
        }
        /// <summary>
        /// 获取打药历史列表（根据区域ID）
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public ActionResult GetDaYaoByAreaId(int areaId)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT tb1.op_date,Pesticides_name
                                                  FROM sprayagricultural tb1 
                                                  left join Pesticides tb2
                                                  on tb1.Pesticides_ID= tb2.Pesticides_ID where area_ID=@areaId 
                                                  order by op_date desc", new { areaId });
                return Success(dt);
            }
        }
        public ActionResult GetCarPoint(int nongjiId)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                var dt = db.Ado.GetDataTable(@"SELECT TOP 1 *
                                                FROM sbpfallT where nongji_ID=@nongjiId order by shebei_date desc",
                                                new { nongjiId });
                return Success(dt);
            }
        }
    }
}