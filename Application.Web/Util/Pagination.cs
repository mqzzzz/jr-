
namespace Application.Web
{
    public class Pagination
    {
        /// <summary>
        /// 每页行数
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 排序列
        /// </summary>
        public string sidx { get; set; }
        /// <summary>
        /// 排序类型
        /// </summary>
        public string sord { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int records { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int total
        {
            get
            {
                if (records > 0)
                {
                    return records % this.rows == 0 ? records / this.rows : records / this.rows + 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 获取分页sql
        /// </summary>
        /// <returns></returns>
        public static string GetPagingSql(string QuerySql, int PageNum, int PageSize)
        {
            string sqltext = "SELECT TOP {0} * FROM (SELECT ROW_NUMBER() ROWNUM,* FROM ({1})A) B where ROWNUM>{2}";
            sqltext = string.Format(sqltext, PageSize, QuerySql, (PageNum - 1) * PageSize);
            return sqltext;
        }
    }

}
