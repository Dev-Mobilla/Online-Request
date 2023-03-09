using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OnlineRequestSystem.Controllers;
using OnlineRequestSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;

namespace OnlineRequestSystem.Service
{
    public class OpenQueries
    {
        private OpenRequestController openRequest_Method = new OpenRequestController();
        private Helper help = new Helper();

        public OpenReqInfo MMD_BranchRequests(ORSession ss, string PO)
        {
            var toTC = new CultureInfo("en-US", false).TextInfo;
            var data = new List<OpenReqViewModel>();
            var Info = new OpenReqInfo();
            var db = new ORtoMySql();

            using (var conn = db.getConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    var intZone = ZoneSelector(ss.s_zonecode);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "openReq_MMD_Branch";
                    cmd.Parameters.AddWithValue("@_zone", intZone);

                    conn.Open();

                    using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        cmd.Parameters.Clear();

                        while (read.Read())
                        {
                            #region Read data
                            var o = new OpenReqViewModel();
                            string typeName = "";

                            o.reqNumber = read["reqNumber"].ToString().Trim();
                            o.reqCreator = read["reqCreator"].ToString().Trim();
                            o.reqDescription = read["reqDescription"].ToString().Trim();
                            o.OverallTotalPrice = read["OverallTotalPrice"].ToString().Trim();
                            o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(read["reqDate"].ToString()));
                            o.TypeID = read["TypeID"].ToString().Trim();
                            o.TotalItems = read["TotalCount"].ToString().Trim();
                            o.itemDescription = read["Description"].ToString().Trim();
                            typeName = help.GetTypeName(o.TypeID);
                            if (typeName.Length > 17)
                            {
                                o.TypeName = typeName.Substring(0, 17) + "..";
                            }
                            else
                            {
                                o.TypeName = typeName;
                            }
                            o.BranchCode = read["BranchCode"].ToString().Trim();
                            o.reqStatus = read["reqStatus"].ToString().Trim();
                            o.ZoneCode = read["Zonecode"].ToString().Trim();
                            o.isDivRequest = read["isDivRequest"].ToString().Trim();
                            o.DeptCode = read["DeptCode"].ToString().Trim();
                            o.Region = read["Region"].ToString().Trim();

                            o.forPresident = read["forPresident"].ToString().Trim();

                            if (!(new[] { "001", "002" }).Contains(o.BranchCode))
                            {
                                o.result_AM = Convert.ToInt32(read["AM"].ToString().Trim());
                                o.result_RM = Convert.ToInt32(read["RM"].ToString().Trim());
                            }

                            o.result_Div1 = Convert.ToInt32(read["Div1"]);
                            o.result_Div2 = Convert.ToInt32(read["Div2"]);
                            o.result_Div3 = Convert.ToInt32(read["Div3"]);
                            o.result_GM = Convert.ToInt32(read["GM"]);
                            o.result_Pres = Convert.ToInt32(read["President"]);
                            o.MMD_Processed = Convert.ToInt32(read["isMMDProcessed"]);
                            o.MMD_ForDelivery = Convert.ToInt32(read["isDelivered"]);
                            o.MMD_InTransit = Convert.ToInt32(read["isMMDTransit"]);
                            o.isApprovedAM = Convert.ToInt32(read["isApprovedAM"]);
                            o.isApprovedRM = Convert.ToInt32(read["isApprovedRM"]);
                            o.isApprovedPres = Convert.ToInt32(read["isApprovedPres"].ToString().Trim());
                            o.reqAM = Convert.ToInt32(read["isAMApproval"]);
                            o.reqRM = Convert.ToInt32(read["isRMApproval"]);
                            o.isApprovedDiv1 = Convert.ToInt32(read["isApprovedDiv1"]);
                            o.isApprovedDiv2 = Convert.ToInt32(read["isApprovedDiv2"]);
                            o.isApprovedDiv3 = Convert.ToInt32(read["isApprovedDiv3"]);
                            o.reqDiv1 = Convert.ToInt32(read["isDivManApproval"]);
                            o.reqDiv2 = Convert.ToInt32(read["isDivManApproval2"]);
                            o.reqDiv3 = Convert.ToInt32(read["isDivManApproval3"]);
                            o.DivCode1 = read["DivCode1"].ToString().Trim();
                            o.DivCode2 = read["DivCode2"].ToString().Trim();
                            o.DivCode3 = read["DivCode3"].ToString().Trim();

                            o.BranchName = toTC.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
                            data.Add(o);

                            #endregion

                        }
                        conn.Close();
                        read.Close();
                    }
                }

                if (PO == "PO")
                {
                    conn.Open();
                    var cmd2 = conn.CreateCommand();

                    foreach (var item in data.ToList())
                    {
                        cmd2.CommandText = "SELECT COUNT(*) AS numOfNotify FROM OnlineRequest.storedComments WHERE reqNumber = @reqNo AND (isViewedBy IS NULL OR isViewedOn IS NULL) AND commCreator = 'Michael L. Lhuillier'";
                        cmd2.Parameters.AddWithValue("@reqNo", item.reqNumber);
                        cmd2.Parameters.AddWithValue("@viewer", ss.s_fullname);
                        using (var read = cmd2.ExecuteReader())
                        {
                            cmd2.Parameters.Clear();
                            read.Read();
                            item.numOfNotifs = Convert.ToInt32(read["numOfNotify"]);                            
                        }
                    }
                }
            }

            Info._OpenInfo = data;
            return Info;
        }

        internal OpenReqInfo MMD_HORequests(ORSession ss, string PO)
        {
            var Info = new OpenReqInfo();
            var db = new ORtoMySql();
            var intZone = ZoneSelector(ss.s_zonecode);
            var data = new List<OpenReqViewModel>();

            using (var conn = db.getConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    var toTC = new CultureInfo("en-US", false).TextInfo;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "openReq_MMD_HOrequests";
                    cmd.Parameters.AddWithValue("@_zone", intZone);
                    cmd.Parameters.AddWithValue("@_costcenter", ss.s_costcenter);
                    conn.Open();
                    var read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (read.Read())
                    {
                        #region Read data
                        var o = new OpenReqViewModel();
                        o.reqNumber = read["reqNumber"].ToString().Trim();
                        o.reqCreator = read["reqCreator"].ToString().Trim();
                        o.reqDescription = read["reqDescription"].ToString().Trim();
                        o.OverallTotalPrice = read["OverallTotalPrice"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(read["reqDate"].ToString()));
                        o.TypeID = read["TypeID"].ToString().Trim();
                        o.TotalItems = read["TotalCount"].ToString().Trim();
                        o.itemDescription = read["Description"].ToString().Trim();
                        string typeName = "";
                        typeName = help.GetTypeName(o.TypeID);
                        if (typeName.Length > 17)
                        {
                            o.TypeName = typeName.Substring(0, 17) + "..";
                        }
                        else
                        {
                            o.TypeName = typeName;
                        }
                        o.BranchCode = read["BranchCode"].ToString().Trim();
                        o.reqStatus = read["reqStatus"].ToString().Trim();
                        o.ZoneCode = read["Zonecode"].ToString().Trim();
                        o.isDivRequest = read["isDivRequest"].ToString().Trim();
                        o.DeptCode = read["DeptCode"].ToString().Trim();
                        o.Region = read["Region"].ToString().Trim();
                        o.Region = openRequest_Method.GetOR_DivisionName(o.DeptCode).ToUpper();
                        o.forPresident = read["forPresident"].ToString().Trim();
                        o.result_DM = Convert.ToInt32(read["DM"]);
                        o.isApprovedLocalDiv = Convert.ToInt32(read["isApprovedLocalDiv"]);

                        o.result_Div1 = Convert.ToInt32(read["Div1"]);
                        o.result_Div2 = Convert.ToInt32(read["Div2"]);
                        o.result_Div3 = Convert.ToInt32(read["Div3"]);
                        o.result_Pres = Convert.ToInt32(read["President"]);

                        o.MMD_Processed = Convert.ToInt32(read["isMMDProcessed"]);
                        o.MMD_ForDelivery = Convert.ToInt32(read["isDelivered"]);
                        o.MMD_InTransit = Convert.ToInt32(read["isMMDTransit"]);

                        o.isApprovedDM = Convert.ToInt32(read["isApprovedDM"]);
                        o.isApprovedPres = Convert.ToInt32(read["isApprovedPres"].ToString().Trim());

                        o.reqDM = Convert.ToInt32(read["isDMApproval"]);
                        o.isApprovedDiv1 = Convert.ToInt32(read["isApprovedDiv1"]);
                        o.isApprovedDiv2 = Convert.ToInt32(read["isApprovedDiv2"]);
                        o.isApprovedDiv3 = Convert.ToInt32(read["isApprovedDiv3"]);

                        o.reqDiv1 = Convert.ToInt32(read["isDivManApproval"]);
                        o.reqDiv2 = Convert.ToInt32(read["isDivManApproval2"]);
                        o.reqDiv3 = Convert.ToInt32(read["isDivManApproval3"]);

                        o.DivCode1 = read["DivCode1"].ToString().Trim();
                        o.DivCode2 = read["DivCode2"].ToString().Trim();
                        o.DivCode3 = read["DivCode3"].ToString().Trim();
                        o.BranchName = getBranchname(o.BranchCode, "HO", o.ZoneCode);
                        data.Add(o);
                        #endregion
                    }

                    conn.Close();
                    read.Close();
                }

                if (PO == "PO")
                {
                    conn.Open();
                    var cmd2 = conn.CreateCommand();

                    foreach (var item in data.ToList())
                    {
                        cmd2.CommandText = "SELECT COUNT(*) AS numOfNotify FROM OnlineRequest.storedComments WHERE reqNumber = @reqNo AND (isViewedBy IS NULL OR isViewedOn IS NULL) AND commCreator = 'Michael L. Lhuillier'";
                        cmd2.Parameters.AddWithValue("@reqNo", item.reqNumber);
                        cmd2.Parameters.AddWithValue("@viewer", ss.s_fullname);
                        using (var read = cmd2.ExecuteReader())
                        {
                            cmd2.Parameters.Clear();
                            read.Read();
                            item.numOfNotifs = Convert.ToInt32(read["numOfNotify"]);
                        }
                    }
                }

                Info._OpenInfo = data;
                return Info;
            }
        }

        internal OpenReqInfo GMO_BranchRequests(ORSession ss)
        {
            var Info = new OpenReqInfo();
            var toTC = new CultureInfo("en-US", false).TextInfo;
            var db = new ORtoMySql();
            var que = new OpenQueries();
            var intZone = ZoneSelector(ss.s_zonecode);
            using (var conn = db.getConnection())
            {
                var OpenReqList = new List<OpenReqViewModel>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "openReq_VP_Branch";
                    cmd.Parameters.AddWithValue("@_zone", intZone);
                    cmd.Parameters.AddWithValue("@_divId", ss.s_DivisionID);
                    conn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            #region Read data
                            var o = new OpenReqViewModel();
                            o.reqNumber = rdr["reqNumber"].ToString().Trim();
                            o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                            o.reqDescription = rdr["reqDescription"].ToString().Trim();
                            o.OverallTotalPrice = rdr["OverallTotalPrice"].ToString().Trim();
                            o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                            o.TypeID = rdr["TypeID"].ToString().Trim();
                            o.TotalItems = rdr["TotalCount"].ToString().Trim();
                            o.itemDescription = rdr["Description"].ToString().Trim();
                            string typeName = "";
                            typeName = help.GetTypeName(o.TypeID);
                            if (typeName.Length > 17)
                            {
                                o.TypeName = typeName.Substring(0, 17) + "..";
                            }
                            else
                            {
                                o.TypeName = typeName;
                            }
                            o.BranchCode = rdr["BranchCode"].ToString().Trim();
                            o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                            o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                            o.reqStatus = rdr["reqStatus"].ToString().Trim();
                            o.DeptCode = rdr["DeptCode"].ToString().Trim();
                            o.Region = rdr["Region"].ToString().Trim().ToUpper();
                            o.forPresident = rdr["forPresident"].ToString().Trim();

                            o.BranchName = toTC.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");

                            if (!(new[] { "001", "002" }).Contains(o.BranchCode))
                            {
                                o.isApprovedAM = Convert.ToInt32(rdr["isApprovedAM"]);
                                o.isApprovedRM = Convert.ToInt32(rdr["isApprovedRM"]);
                                o.reqAM = Convert.ToInt32(rdr["isAMApproval"]);
                                o.reqRM = Convert.ToInt32(rdr["isRMApproval"]);
                            }

                            o.isApprovedGM = Convert.ToInt32(rdr["isApprovedGM"]);
                            o.isApprovedVPAssistant = Convert.ToInt32(rdr["isApprovedVPAssistant"]);
                            o.isApprovedDiv1 = Convert.ToInt32(rdr["isApprovedDiv1"]);
                            o.isApprovedDiv2 = Convert.ToInt32(rdr["isApprovedDiv2"]);
                            o.isApprovedDiv3 = Convert.ToInt32(rdr["isApprovedDiv3"]);
                            o.isApprovedPres = Convert.ToInt32(rdr["isApprovedPres"]);
                            o.DivCode1 = rdr["DivCode1"].ToString().Trim();
                            o.DivCode2 = rdr["DivCode2"].ToString().Trim();
                            o.DivCode3 = rdr["DivCode3"].ToString().Trim();
                            o.reqGM = Convert.ToInt32(rdr["isGMApproval"]);
                            o.reqDiv1 = Convert.ToInt32(rdr["isDivManApproval"]);
                            o.reqDiv2 = Convert.ToInt32(rdr["isDivManApproval2"]);
                            o.reqDiv3 = Convert.ToInt32(rdr["isDivManApproval3"]);
                            o.reqPres = Convert.ToInt32(rdr["isPresidentApproval"]);
                            o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                            o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                            o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);

                            OpenReqList.Add(o);
                            #endregion
                        }
                    }
                    Info._OpenInfo = OpenReqList;
                }
            }
            return Info;
        }

        internal OpenReqInfo GMO_HORequests(ORSession ss)
        {
            var Info = new OpenReqInfo();
            var toTC = new CultureInfo("en-US", false).TextInfo;
            var db = new ORtoMySql();
            var que = new OpenQueries();
            var intZone = ZoneSelector(ss.s_zonecode);
            using (var conn = db.getConnection())
            {
                var OpenReqList = new List<OpenReqViewModel>();
                var cmd = conn.CreateCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "openReq_VP_HO";
                cmd.Parameters.AddWithValue("@_zone", intZone);
                cmd.Parameters.AddWithValue("@_divId", ss.s_DivisionID);

                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        #region Read data

                        var o = new OpenReqViewModel();
                        o.reqNumber = rdr["reqNumber"].ToString().Trim();
                        o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                        o.reqDescription = rdr["reqDescription"].ToString().Trim();
                        o.OverallTotalPrice = rdr["OverallTotalPrice"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                        o.TypeID = rdr["TypeID"].ToString().Trim();
                        o.TotalItems = rdr["TotalCount"].ToString().Trim();
                        o.itemDescription = rdr["Description"].ToString().Trim();
                        string typeName = "";
                        typeName = help.GetTypeName(o.TypeID);
                        if (typeName.Length > 17)
                        {
                            o.TypeName = typeName.Substring(0, 17) + "..";
                        }
                        else
                        {
                            o.TypeName = typeName;
                        }
                        o.BranchCode = rdr["BranchCode"].ToString().Trim();
                        o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                        o.reqStatus = rdr["reqStatus"].ToString().Trim();
                        o.DeptCode = rdr["DeptCode"].ToString().Trim();
                        o.Region = rdr["Region"].ToString().Trim().ToUpper();
                        o.Region = openRequest_Method.GetOR_DivisionName(o.DeptCode).ToUpper();
                        o.forPresident = rdr["forPresident"].ToString().Trim();
                        o.BranchName = getBranchname(o.BranchCode, "HO", o.ZoneCode);

                        o.isApprovedLocalDiv = Convert.ToInt32(rdr["isApprovedLocalDiv"]);
                        o.isApprovedDM = Convert.ToInt32(rdr["isApprovedDM"]);
                        o.isApprovedGM = Convert.ToInt32(rdr["isApprovedGM"]);
                        o.isApprovedVPAssistant = Convert.ToInt32(rdr["isApprovedVPAssistant"]);
                        o.isApprovedDiv1 = Convert.ToInt32(rdr["isApprovedDiv1"]);
                        o.isApprovedDiv2 = Convert.ToInt32(rdr["isApprovedDiv2"]);
                        o.isApprovedDiv3 = Convert.ToInt32(rdr["isApprovedDiv3"]);
                        o.isApprovedPres = Convert.ToInt32(rdr["isApprovedPres"]);

                        o.DivCode1 = rdr["DivCode1"].ToString().Trim();
                        o.DivCode2 = rdr["DivCode2"].ToString().Trim();
                        o.DivCode3 = rdr["DivCode3"].ToString().Trim();
                        o.reqDM = Convert.ToInt32(rdr["isDMApproval"]);
                        o.reqGM = Convert.ToInt32(rdr["isGMApproval"]);
                        o.reqDiv1 = Convert.ToInt32(rdr["isDivManApproval"]);
                        o.reqDiv2 = Convert.ToInt32(rdr["isDivManApproval2"]);
                        o.reqDiv3 = Convert.ToInt32(rdr["isDivManApproval3"]);
                        o.reqPres = Convert.ToInt32(rdr["isPresidentApproval"]);
                        o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                        o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                        o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);

                        #endregion Read data from database

                        OpenReqList.Add(o);
                    }
                }
                Info._OpenInfo = OpenReqList;
            }
            return new OpenReqInfo { _OpenInfo = Info._OpenInfo };
        }

        public string getBranchname(string BranchCode, string Region, string ZoneCode)
        {
            string b = "";
            try
            {
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                Uri source = new System.Uri(w + "/GetBranchName?BranchCode=" + BranchCode + "&Region=" + Region + "&ZoneCode=" + ZoneCode);
                RequestHandler reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return "Service Unavailable";

                var Response = JsonConvert.DeserializeObject<BranchNameResponse>(x);
                b = Response.BranchName;
            }
            catch (Exception)
            {
                return b;
            }
            return b;
        }

        public OpenReqInfo HeadOfficeRequests(ORSession ss, string approver)
        {
            var Info = new OpenReqInfo();
            var toTC = new CultureInfo("en-US", false).TextInfo;
            var db = new ORtoMySql();
            var que = new OpenQueries();
            var intZone = ZoneSelector(ss.s_zonecode);
            using (var conn = db.getConnection())
            {
                var OpenReqList = new List<OpenReqViewModel>();
                var cmd = conn.CreateCommand();
                switch (approver)
                {
                    case "Div":
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "openReq_Div_HOreq";
                        cmd.Parameters.AddWithValue("@_DivID", ss.s_DivisionID);
                        cmd.Parameters.AddWithValue("@_zone", intZone);
                        cmd.Parameters.AddWithValue("@_costcenter", ss.s_costcenter);
                        break;

                    default:
                        break;
                }
                conn.Open();
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        #region Read data from database

                        var o = new OpenReqViewModel();
                        o.reqNumber = rdr["reqNumber"].ToString().Trim();
                        o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                        o.reqDescription = rdr["reqDescription"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                        o.TypeID = rdr["TypeID"].ToString().Trim();
                        o.TotalItems = Convert.ToString(rdr["TotalCount"].ToString().Trim());
                        o.itemDescription = rdr["Description"].ToString().Trim();
                        string typeName = "";
                        typeName = help.GetTypeName(o.TypeID);
                        if (typeName.Length > 17)
                        {
                            o.TypeName = typeName.Substring(0, 17) + "..";
                        }
                        else
                        {
                            o.TypeName = typeName;
                        }
                        o.BranchCode = rdr["BranchCode"].ToString().Trim();
                        o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                        o.reqStatus = rdr["reqStatus"].ToString().Trim();
                        o.DeptCode = rdr["DeptCode"].ToString().Trim();
                        o.Region = rdr["Region"].ToString().Trim().ToUpper();
                        o.Region = openRequest_Method.GetOR_DivisionName(o.DeptCode).ToUpper();
                        o.forPresident = rdr["forPresident"].ToString().Trim();
                        o.BranchName = getBranchname(o.BranchCode, "HO", o.ZoneCode);

                        o.isApprovedLocalDiv = Convert.ToInt32(rdr["isApprovedLocalDiv"]);
                        o.isApprovedDM = Convert.ToInt32(rdr["isApprovedDM"]);
                        o.isApprovedGM = Convert.ToInt32(rdr["isApprovedGM"]);
                        o.isApprovedVPAssistant = Convert.ToInt32(rdr["isApprovedVPAssistant"]);
                        o.isApprovedDiv1 = Convert.ToInt32(rdr["isApprovedDiv1"]);
                        o.isApprovedDiv2 = Convert.ToInt32(rdr["isApprovedDiv2"]);
                        o.isApprovedDiv3 = Convert.ToInt32(rdr["isApprovedDiv3"]);
                        o.isApprovedPres = Convert.ToInt32(rdr["isApprovedPres"]);

                        o.DivCode1 = rdr["DivCode1"].ToString().Trim();
                        o.DivCode2 = rdr["DivCode2"].ToString().Trim();
                        o.DivCode3 = rdr["DivCode3"].ToString().Trim();
                        o.reqDM = Convert.ToInt32(rdr["isDMApproval"]);
                        o.reqGM = Convert.ToInt32(rdr["isGMApproval"]);
                        o.reqDiv1 = Convert.ToInt32(rdr["isDivManApproval"]);
                        o.reqDiv2 = Convert.ToInt32(rdr["isDivManApproval2"]);
                        o.reqDiv3 = Convert.ToInt32(rdr["isDivManApproval3"]);
                        o.reqPres = Convert.ToInt32(rdr["isPresidentApproval"]);
                        o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                        o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                        o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);

                        #endregion Read data from database

                        OpenReqList.Add(o);
                    }
                }
                Info._OpenInfo = OpenReqList;
            }
            return new OpenReqInfo { _OpenInfo = Info._OpenInfo };
        }

        public OpenReqInfo BranchRequests(ORSession ss, string approver)
        {
            var Info = new OpenReqInfo();
            var toTC = new CultureInfo("en-US", false).TextInfo;
            var db = new ORtoMySql();
            var que = new OpenQueries();
            var intZone = ZoneSelector(ss.s_zonecode);
            using (var conn = db.getConnection())
            {
                var OpenReqList = new List<OpenReqViewModel>();
                var cmd = conn.CreateCommand();
                switch (approver)
                {
                    case "Div":
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "openReq_Div_BranchReq";
                        cmd.Parameters.AddWithValue("@_DivID", ss.s_DivisionID);
                        cmd.Parameters.AddWithValue("@_zone", intZone);
                        break;

                    default:
                        break;
                }
                conn.Open();
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        #region Read data from database

                        var o = new OpenReqViewModel();
                        o.reqNumber = rdr["reqNumber"].ToString().Trim();
                        o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                        o.reqDescription = rdr["reqDescription"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                        o.TypeID = rdr["TypeID"].ToString().Trim();
                        o.TotalItems = Convert.ToString(rdr["TotalCount"].ToString().Trim());
                        o.itemDescription = rdr["Description"].ToString().Trim();
                        string typeName = "";
                        typeName = help.GetTypeName(o.TypeID);
                        if (typeName.Length > 17)
                        {
                            o.TypeName = typeName.Substring(0, 17) + "..";
                        }
                        else
                        {
                            o.TypeName = typeName;
                        }
                        o.BranchCode = rdr["BranchCode"].ToString().Trim();
                        o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                        o.reqStatus = rdr["reqStatus"].ToString().Trim();
                        o.DeptCode = rdr["DeptCode"].ToString().Trim();
                        o.Region = rdr["Region"].ToString().Trim().ToUpper();

                        o.forPresident = rdr["forPresident"].ToString().Trim();

                        o.BranchName = toTC.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
                        if (!(new[] { "001", "002" }).Contains(o.BranchCode))
                        {
                            o.isApprovedAM = Convert.ToInt32(rdr["isApprovedAM"]);
                            o.isApprovedRM = Convert.ToInt32(rdr["isApprovedRM"]);
                            o.reqAM = Convert.ToInt32(rdr["isAMApproval"]);
                            o.reqRM = Convert.ToInt32(rdr["isRMApproval"]);
                        }

                        o.isApprovedGM = Convert.ToInt32(rdr["isApprovedGM"]);
                        o.isApprovedVPAssistant = Convert.ToInt32(rdr["isApprovedVPAssistant"]);
                        o.isApprovedDiv1 = Convert.ToInt32(rdr["isApprovedDiv1"]);
                        o.isApprovedDiv2 = Convert.ToInt32(rdr["isApprovedDiv2"]);
                        o.isApprovedDiv3 = Convert.ToInt32(rdr["isApprovedDiv3"]);
                        o.isApprovedPres = Convert.ToInt32(rdr["isApprovedPres"]);
                        o.DivCode1 = rdr["DivCode1"].ToString().Trim();
                        o.DivCode2 = rdr["DivCode2"].ToString().Trim();
                        o.DivCode3 = rdr["DivCode3"].ToString().Trim();
                        o.reqGM = Convert.ToInt32(rdr["isGMApproval"]);
                        o.reqDiv1 = Convert.ToInt32(rdr["isDivManApproval"]);
                        o.reqDiv2 = Convert.ToInt32(rdr["isDivManApproval2"]);
                        o.reqDiv3 = Convert.ToInt32(rdr["isDivManApproval3"]);
                        o.reqPres = Convert.ToInt32(rdr["isPresidentApproval"]);
                        o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                        o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                        o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);

                        #endregion Read data from database

                        OpenReqList.Add(o);
                    }
                }
                Info._OpenInfo = OpenReqList;
            }
            return new OpenReqInfo { _OpenInfo = Info._OpenInfo };
        }

        internal OpenReqInfo MyRequests(ORSession ss)
        {
            var toTC = new CultureInfo("en-US", false).TextInfo;
            var Info = new OpenReqInfo();
            var OpenReqList = new List<OpenReqViewModel>();
            var db = new ORtoMySql();
            var que = new OpenQueries();
            var intZone = ZoneSelector(ss.s_zonecode);
            using (var conn = db.getConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "openReq_MyRequests";
                    cmd.Parameters.AddWithValue("@_usrid", ss.s_usr_id);

                    conn.Open();
                    var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (rdr.Read())
                    {
                        #region Read data from database

                        var o = new OpenReqViewModel();
                        o.reqNumber = rdr["reqNumber"].ToString().Trim();
                        o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                        o.reqDescription = rdr["reqDescription"].ToString().Trim();
                        o.OverallTotalPrice = rdr["OverallTotalPrice"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                        o.TypeID = rdr["TypeID"].ToString().Trim();
                        o.TotalItems = rdr["TotalCount"].ToString().Trim();
                        o.itemDescription = rdr["Description"].ToString().Trim();
                        string typeName = "";
                        typeName = help.GetTypeName(o.TypeID);
                        if (typeName.Length > 17)
                        {
                            o.TypeName = typeName.Substring(0, 17) + "..";
                        }
                        else
                        {
                            o.TypeName = typeName;
                        }
                        o.BranchCode = rdr["BranchCode"].ToString().Trim();
                        o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                        o.reqStatus = rdr["reqStatus"].ToString().Trim();
                        o.DeptCode = rdr["DeptCode"].ToString().Trim();
                        o.Region = rdr["Region"].ToString().Trim().ToUpper();
                        if (o.Region == "HO")
                        {
                            o.Region = openRequest_Method.GetOR_DivisionName(o.DeptCode).ToUpper();
                        }
                        o.forPresident = rdr["forPresident"].ToString().Trim();
                        if (o.isDivRequest == "1")
                        {
                            o.BranchName = getBranchname(o.BranchCode, "HO", o.ZoneCode);
                        }
                        else
                        {
                            o.BranchName = toTC.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
                        }

                        if (!(new[] { "001", "002" }).Contains(o.BranchCode))
                        {
                            o.isApprovedAM = Convert.ToInt32(rdr["isApprovedAM"]);
                            o.isApprovedRM = Convert.ToInt32(rdr["isApprovedRM"]);
                            o.reqAM = Convert.ToInt32(rdr["isAMApproval"]);
                            o.reqRM = Convert.ToInt32(rdr["isRMApproval"]);
                        }
                        o.isApprovedLocalDiv = Convert.ToInt32(rdr["isApprovedLocalDiv"]);
                        o.isApprovedDM = Convert.ToInt32(rdr["isApprovedDM"]);
                        o.isApprovedGM = Convert.ToInt32(rdr["isApprovedGM"]);
                        o.isApprovedVPAssistant = Convert.ToInt32(rdr["isApprovedVPAssistant"]);
                        o.isApprovedDiv1 = Convert.ToInt32(rdr["isApprovedDiv1"]);
                        o.isApprovedDiv2 = Convert.ToInt32(rdr["isApprovedDiv2"]);
                        o.isApprovedDiv3 = Convert.ToInt32(rdr["isApprovedDiv3"]);
                        o.isApprovedPres = Convert.ToInt32(rdr["isApprovedPres"]);

                        o.DivCode1 = rdr["DivCode1"].ToString().Trim();
                        o.DivCode2 = rdr["DivCode2"].ToString().Trim();
                        o.DivCode3 = rdr["DivCode3"].ToString().Trim();
                        o.reqDM = Convert.ToInt32(rdr["isDMApproval"]);
                        o.reqGM = Convert.ToInt32(rdr["isGMApproval"]);
                        o.reqDiv1 = Convert.ToInt32(rdr["isDivManApproval"]);
                        o.reqDiv2 = Convert.ToInt32(rdr["isDivManApproval2"]);
                        o.reqDiv3 = Convert.ToInt32(rdr["isDivManApproval3"]);
                        o.reqPres = Convert.ToInt32(rdr["isPresidentApproval"]);
                        o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                        o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                        o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);

                        #endregion Read data from database

                        OpenReqList.Add(o);
                    }
                    conn.Close();
                    rdr.Close();
                }
                conn.Open();
                var cmd2 = conn.CreateCommand();

                foreach (var item in OpenReqList.ToList())
                {
                    cmd2.CommandText = "SELECT COUNT(*) AS numOfNotify FROM OnlineRequest.storedComments WHERE reqNumber = @reqNo AND (isViewedBy IS NULL OR isViewedOn IS NULL) AND commCreator = 'Michael L. Lhuillier'";
                    cmd2.Parameters.AddWithValue("@reqNo", item.reqNumber);
                    cmd2.Parameters.AddWithValue("@viewer", ss.s_fullname);
                    using (var read = cmd2.ExecuteReader())
                    {
                        cmd2.Parameters.Clear();
                        read.Read();
                        item.numOfNotifs = Convert.ToInt32(read["numOfNotify"]);
                        OpenReqList.Add(item);
                    }
                }
                Info._OpenInfo = OpenReqList;
                return new OpenReqInfo { _OpenInfo = Info._OpenInfo };
        }
    }

        public OpenReqInfo LocalRequests(ORSession ss, string div_dept)
        {
            var Info = new OpenReqInfo();
            var OpenReqList = new List<OpenReqViewModel>();
            var toTC = new CultureInfo("en-US", false).TextInfo;
            var db = new ORtoMySql();
            var que = new OpenQueries();
            var intZone = ZoneSelector(ss.s_zonecode);
            using (var conn = db.getConnection())
            {

                using (var cmd = conn.CreateCommand())
                {
                    string storedProc = "";

                    if (div_dept == "div")
                    {
                        storedProc = "openReq_LocalRequests";
                    }
                    else if (div_dept == "dept")
                    {
                        storedProc = "openReq_LocalRequestsDept";
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = storedProc;
                    cmd.Parameters.AddWithValue("@_costcenter", ss.s_costcenter);
                    cmd.Parameters.AddWithValue("@_zonecode", intZone);
                    if (ss.s_DivisionID == null)
                    {
                        ss.s_DivisionID = string.Empty;
                    }
                    cmd.Parameters.AddWithValue("@_divId", ss.s_DivisionID);

                    conn.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            #region Read data from database

                            var o = new OpenReqViewModel();
                            o.reqNumber = rdr["reqNumber"].ToString().Trim();
                            o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                            o.reqDescription = rdr["reqDescription"].ToString().Trim();
                            o.OverallTotalPrice = rdr["OverallTotalPrice"].ToString().Trim();
                            o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                            o.TypeID = rdr["TypeID"].ToString().Trim();
                            o.TotalItems = rdr["TotalCount"].ToString().Trim();
                            o.itemDescription = rdr["Description"].ToString().Trim();

                            string typeName = "";
                            typeName = help.GetTypeName(o.TypeID);
                            if (typeName.Length > 17)
                            {
                                o.TypeName = typeName.Substring(0, 17) + "..";
                            }
                            else
                            {
                                o.TypeName = typeName;
                            }

                            o.BranchCode = rdr["BranchCode"].ToString().Trim();
                            o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                            o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                            o.reqStatus = rdr["reqStatus"].ToString().Trim();
                            o.DeptCode = rdr["DeptCode"].ToString().Trim();
                            o.Region = rdr["Region"].ToString().Trim().ToUpper();
                            if (o.Region == "HO")
                            {
                                o.Region = openRequest_Method.GetOR_DivisionName(o.DeptCode).ToUpper();
                            }
                            o.forPresident = rdr["forPresident"].ToString().Trim();
                            if (o.isDivRequest == "1")
                            {
                                o.BranchName = getBranchname(o.BranchCode, "HO", o.ZoneCode);
                            }
                            else
                            {
                                o.BranchName = toTC.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
                            }

                            if (!(new[] { "001", "002" }).Contains(o.BranchCode))
                            {
                                o.isApprovedAM = Convert.ToInt32(rdr["isApprovedAM"]);
                                o.isApprovedRM = Convert.ToInt32(rdr["isApprovedRM"]);
                                o.reqAM = Convert.ToInt32(rdr["isAMApproval"]);
                                o.reqRM = Convert.ToInt32(rdr["isRMApproval"]);
                            }
                            o.isApprovedLocalDiv = Convert.ToInt32(rdr["isApprovedLocalDiv"]);
                            o.isApprovedDM = Convert.ToInt32(rdr["isApprovedDM"]);
                            o.isApprovedGM = Convert.ToInt32(rdr["isApprovedGM"]);
                            o.isApprovedVPAssistant = Convert.ToInt32(rdr["isApprovedVPAssistant"]);
                            o.isApprovedDiv1 = Convert.ToInt32(rdr["isApprovedDiv1"]);
                            o.isApprovedDiv2 = Convert.ToInt32(rdr["isApprovedDiv2"]);
                            o.isApprovedDiv3 = Convert.ToInt32(rdr["isApprovedDiv3"]);
                            o.isApprovedPres = Convert.ToInt32(rdr["isApprovedPres"]);

                            o.DivCode1 = rdr["DivCode1"].ToString().Trim();
                            o.DivCode2 = rdr["DivCode2"].ToString().Trim();
                            o.DivCode3 = rdr["DivCode3"].ToString().Trim();
                            o.reqDM = Convert.ToInt32(rdr["isDMApproval"]);
                            o.reqGM = Convert.ToInt32(rdr["isGMApproval"]);
                            o.reqDiv1 = Convert.ToInt32(rdr["isDivManApproval"]);
                            o.reqDiv2 = Convert.ToInt32(rdr["isDivManApproval2"]);
                            o.reqDiv3 = Convert.ToInt32(rdr["isDivManApproval3"]);
                            o.reqPres = Convert.ToInt32(rdr["isPresidentApproval"]);
                            o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                            o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                            o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);

                            #endregion Read data from database

                            OpenReqList.Add(o);
                        }
                        conn.Close();
                        rdr.Close();
                    }
                }

                conn.Open();
                var cmd2 = conn.CreateCommand();

                foreach (var item in OpenReqList.ToList())
                {
                    cmd2.CommandText = "SELECT COUNT(*) AS numOfNotify FROM OnlineRequest.storedComments WHERE reqNumber = @reqNo AND (isViewedBy IS NULL OR isViewedOn IS NULL) AND commCreator = 'Michael L. Lhuillier'";
                    cmd2.Parameters.AddWithValue("@reqNo", item.reqNumber);
                    cmd2.Parameters.AddWithValue("@viewer", ss.s_fullname);
                    using (var read = cmd2.ExecuteReader())
                    {
                        cmd2.Parameters.Clear();
                        read.Read();
                        item.numOfNotifs = Convert.ToInt32(read["numOfNotify"]);
                        OpenReqList.Add(item);
                    }
                }
            }

            Info._OpenInfo = OpenReqList;
            return new OpenReqInfo { _OpenInfo = Info._OpenInfo };
        }
            


        private string ZoneSelector(string zone)
        {
            if ((new[] { "VISMIN", "VISAYAS", "MINDANAO" }).Contains(zone))
            { return "V"; }
            else
            { return "L"; }
        }

        //public status_filter(string zoneCode, string branchCode)
        //{
        //    var db = new ORtoMySql();

        //    using (MySqlConnection con = db.getConnection())
        //    {
        //        using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `OnlineRequest`.`requestType` WHERE `isGMApproval`='1' OR `isPresidentApproval` = '1'", con))
        //        {
        //            con.Open();
        //            MySqlDataReader rdr = cmd.ExecuteReader();

        //            if (rdr.HasRows)
        //            {
        //                con.Close();
        //                return true;
        //            }
        //            else
        //            {
        //                con.Close();
        //                return false;
        //            }
        //        }
        //    }
        //}
    }
}