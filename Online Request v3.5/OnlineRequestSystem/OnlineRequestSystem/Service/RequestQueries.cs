using MySql.Data.MySqlClient;
using OnlineRequestSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace OnlineRequestSystem.Service
{
    public class RequestQueries
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RequestQueries));

        public string GetRequestNumber()
        {
            int RequestNumber = 0;
            string ReturnValue = "";
            ORtoMySql db = new ORtoMySql();
            using (MySqlConnection conn = db.getConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    String SQL;
                    SQL = " SELECT reqNumber AS reqNumber " +
                          " FROM (SELECT MAX(SUBSTR(reqNumber, LENGTH(reqNumber) -4, LENGTH(reqNumber))) AS reqNumber FROM OnlineRequest.onlineRequest_Open " +
                          " UNION " +
                          " SELECT MAX(SUBSTR(reqNumber, LENGTH(reqNumber) -4, LENGTH(reqNumber)))  AS reqNumber FROM OnlineRequest.onlineRequest_Close) v " +
                          " ORDER BY reqNumber DESC LIMIT 1";
                    cmd.CommandText = SQL;
                    cmd.CommandTimeout = 0;

                    using (MySqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (read.HasRows)
                        {
                            if (read.Read())
                            {
                                var fromdb = read["reqNumber"].ToString().Trim();
                                if (fromdb == "")
                                {
                                    RequestNumber = 00001;
                                }
                                else
                                {
                                    RequestNumber = Convert.ToInt32(fromdb) + 1;
                                }

                                ReturnValue = RequestNumber.ToString();
                            }
                        }
                        return ReturnValue.PadLeft(5, '0');
                    }
                }
            }
        }

        public string UpdateRecommendedApproval(MySqlConnection con, MySqlCommand cmd, ORtoMySql db, MySqlTransaction tran, string type)
        {
            string Resp = "";
            try
            {
                object divecodeResult = "";

                con = db.getConnection();
                cmd = con.CreateCommand();

                cmd.CommandText = " UPDATE OnlineRequest.requestType SET isDMApproval = @deptApproval WHERE TypeID = @typeid ";
                cmd.Parameters.AddWithValue("@deptApproval", 1);
                cmd.Parameters.AddWithValue("@typeid", type);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();

                Resp = "success";
            }
            catch (Exception x)
            {
                tran.Rollback();
                log.Fatal(x.Message, x);
                Resp = "error";
            }
            return Resp;
        }

        public Boolean RequestNumberChecking(string ReqNo)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                cmd.CommandText = " SELECT * FROM OnlineRequest.onlineRequest_Open WHERE reqNumber = @ReqNo";
                cmd.CommandTimeout = 0;
                conn.Open();
                using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (read.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private List<RequestTypeMenu> RequestMenu()
        {
            var menu = new List<RequestTypeMenu>();
            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = " SELECT id, reqType, isAMApproval, isRMApproval, isGMApproval, isDivManApproval, isPresidentApproval FROM OnlineRequest.requestType";
                    conn.Open();
                    var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (rdr.Read())
                    {
                        var m = new RequestTypeMenu();
                        m.IDforUpdate = Convert.ToInt32(rdr["id"].ToString().Trim());
                        m.RequestType = rdr["reqType"].ToString().Trim();
                        m.isAMApproval = Convert.ToInt32(rdr["isAMApproval"].ToString().Trim());
                        m.isRMApproval = Convert.ToInt32(rdr["isRMApproval"].ToString().Trim());
                        m.isGMApproval = Convert.ToInt32(rdr["isGMApproval"].ToString().Trim());
                        m.isDivManApproval = Convert.ToInt32(rdr["isDivManApproval"].ToString().Trim());
                        m.isPresidentApproval = Convert.ToInt32(rdr["isPresidentApproval"].ToString().Trim());
                        menu.Add(m);
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
            return menu;
        }

        public List<SelectListItem> ListOfReqType(string zonecode)
        {
            var ListItem = new List<SelectListItem>();
            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        if ((new[] { "LNCR", "LUZON", "NCR" }).Contains(zonecode))
                        {
                            zonecode = "LUZON";
                        }
                        else
                        {
                            zonecode = "VISMIN";
                        }
                        String x = " SELECT * FROM OnlineRequest.requestType WHERE ZoneCode = @zcode";
                        cmd.Parameters.AddWithValue("@zcode", zonecode);
                        cmd.CommandText = x;
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())
                            {
                                ListItem.Add(new SelectListItem { Text = read["reqType"].ToString().Trim(), Value = read["TypeID"].ToString().Trim() });
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
            return ListItem;
        }
    }
}