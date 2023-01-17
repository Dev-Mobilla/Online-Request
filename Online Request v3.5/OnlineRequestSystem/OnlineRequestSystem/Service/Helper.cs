using MySql.Data.MySqlClient;
using OnlineRequestSystem.Models;
using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace OnlineRequestSystem.Service
{
    public class Helper
    {
        private ORtoMySql db = new ORtoMySql();
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;
        private string format = "yyyy/MM/dd HH:mm:ss";

        public DateTime ServerDate()
        {
            var con = db.getConnection();
            var date = DateTime.Now;
            using (con) 
            {
                string sql = "SELECT NOW() AS ServerDate";
                var cmd = new MySqlCommand(sql, con);
                con.Open();
                var read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                {
                    if (read.Read())
                    {
                        date = Convert.ToDateTime(read["ServerDate"]);
                    }
                }
                return date;
            }
        }

        internal string GetDepartmentManager(string CostCenter)
        {
            string rvalue = "";
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    string Query = "SELECT deptManager FROM OnlineRequest.department WHERE CostCenter = @CostCenter";
                    cmd.Parameters.AddWithValue("@CostCenter", CostCenter);
                    cmd.CommandText = Query;
                    cmd.CommandTimeout = 0;
                    using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (read.HasRows)
                        {
                            read.Read();
                            rvalue = read["deptManager"].ToString().Trim();
                        }
                    }
                    return rvalue;
                }
            }
        }

        internal void SetDateModified(string ReqNo, string sysmodifier)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = " UPDATE OnlineRequest.onlineRequest_Open SET sysmodified = @date, " +
                                  " sysmodifier = @modifier WHERE reqNumber = @ReqNo";
                cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                cmd.Parameters.AddWithValue("@modifier", sysmodifier);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture).ToString());
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        internal string GetTypeName(string p)
        {
            string rvalue = "";
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.Parameters.AddWithValue("@TypeID", p);
                cmd.CommandText = "SELECT reqType FROM OnlineRequest.requestType WHERE TypeID = @TypeID";
                using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (read.HasRows)
                    {
                        read.Read();
                        rvalue = read["reqType"].ToString().Trim();
                    }
                }
                return rvalue;
            }
        }

        internal string ZoneCodeSelector(string zonecode)
        {
            string x = "";
            if ((new[] { "LUZON", "LNCR", "NCR" }).Contains(zonecode))
            {
                x = "LUZON";
            }
            else
            {
                x = "VISMIN";
            }
            return x;
        }

        internal int DiagnosticCheck(string ReqNo)
        {
            int x = 0;
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();       
                cmd.Parameters.AddWithValue("@req", ReqNo);
                cmd.CommandText = "SELECT diagnostic FROM diagnosticFiles WHERE reqNumber = @req";
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (rdr.HasRows)
                    {
                        x = 1;
                    }
                    else
                    {
                        x = 0;
                    }
                }
            }
            return x;
        }
        
    }
}