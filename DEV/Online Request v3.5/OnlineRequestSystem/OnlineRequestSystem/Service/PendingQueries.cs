using MySql.Data.MySqlClient;
using OnlineRequestSystem.Controllers;
using OnlineRequestSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace OnlineRequestSystem.Service
{
    public class PendingQueries
    {
        private OpenRequestController oReq = new OpenRequestController();
        private Helper help = new Helper();

        public OpenReqInfo Pending_MMD_Vismin(ORSession ss, OpenReqInfo Info)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var data = new List<OpenReqViewModel>();

                var cmd = conn.CreateCommand();
                string MMD = " SELECT " +
                             " a.reqNumber,  a.reqCreator , a.reqDescription, COUNT(d.itemDescription) AS TotalCount," +
                             " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                             " a.reqDate, a.TypeID , a.reqTotal, a.BranchCode, a.Region, a.DivCode, a.Zonecode, a.reqStatus, " +
                             " a.isDivRequest, a.DeptCode, a.forPresident," +
                             " IF(b.isApprovedDM = c.isDMApproval, 1,0) AS DM, " +
                             " IF(b.isApprovedAM = c.isAMApproval,1,0) AS AM, " +
                             " IF(b.isApprovedRM = c.isRMApproval, 1, 0) AS RM, " +
                             " IF(b.isApprovedGM = c.isGMApproval, 1, 0) AS GM, b.isApprovedVPAssistant, " +
                             " IF(b.isApprovedDiv1 = c.isDivManApproval, 1, 0) AS Div1, " +
                             " IF(b.isApprovedDiv2 = c.isDivManApproval2, 1, 0) AS Div2, " +
                             " IF(b.isApprovedDiv3 = c.isDivManApproval3, 1, 0) AS Div3, " +
                             " IF(b.isApprovedPres = c.isPresidentApproval, 1, 0) AS President, " +
                             " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit, b.isApprovedDiv1, b.isApprovedDM, c.isDMApproval, b.isApprovedLocalDiv, b.isApprovedAM, c.isAMApproval, b.isApprovedRM, c.isRMApproval," +
                             " b.isApprovedPres, c.isDivManApproval, b.isApprovedDiv2, c.isDivManApproval2, b.isApprovedDiv3, c.isDivManApproval3,c.DivCode1, c.DivCode2, c.DivCode3 " +
                             " FROM onlineRequest_Open a " +
                             " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                             " INNER JOIN requestType c ON a.TypeID = c.TypeID " +
                             " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber  " +
                             " WHERE a.TypeID = c.TypeID AND a.reqStatus = 'PENDING' AND (a.ZoneCode = 'VISMIN' OR a.ZoneCode = 'VISAYAS' OR a.ZoneCode = 'MINDANAO') " +
                             " GROUP BY a.reqNumber ORDER BY a.syscreated ASC";
                cmd.CommandText = MMD;
                conn.Open();
                var read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (read.Read())
                {
                    var o = new OpenReqViewModel();
                    o.reqNumber = read["reqNumber"].ToString().Trim();
                    o.reqCreator = read["reqCreator"].ToString().Trim();
                    o.reqDescription = read["reqDescription"].ToString().Trim();
                    o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(read["reqDate"].ToString()));
                    o.TypeID = read["TypeID"].ToString().Trim();

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

                    o.TotalItems = read["TotalCount"].ToString().Trim();
                    o.itemDescription = read["Description"].ToString().Trim();
                    o.BranchCode = read["BranchCode"].ToString().Trim();
                    o.ZoneCode = read["Zonecode"].ToString().Trim();
                    o.isDivRequest = read["isDivRequest"].ToString().Trim();
                    o.DeptCode = read["DeptCode"].ToString().Trim();
                    o.Region = read["Region"].ToString().Trim();
                    if (o.Region == "HO")
                    {
                        o.Region = oReq.GetOR_DivisionName(o.DeptCode).ToUpper();
                    }
                    o.forPresident = read["forPresident"].ToString().Trim();

                    if (!(new[] { "001", "002" }).Contains(o.BranchCode))
                    {
                        o.result_AM = Convert.ToInt32(read["AM"].ToString().Trim());
                        o.result_RM = Convert.ToInt32(read["RM"].ToString().Trim());
                    }

                    o.result_DM = Convert.ToInt32(read["DM"]);
                    o.isApprovedLocalDiv = Convert.ToInt32(read["isApprovedLocalDiv"]);

                    o.result_Div1 = Convert.ToInt32(read["Div1"]);
                    o.result_Div2 = Convert.ToInt32(read["Div2"]);
                    o.result_Div3 = Convert.ToInt32(read["Div3"]);
                    o.result_GM = Convert.ToInt32(read["GM"]);
                    o.result_Pres = Convert.ToInt32(read["President"]);

                    o.MMD_Processed = Convert.ToInt32(read["isMMDProcessed"]);
                    o.MMD_ForDelivery = Convert.ToInt32(read["isDelivered"]);
                    o.MMD_InTransit = Convert.ToInt32(read["isMMDTransit"]);

                    o.isApprovedDM = Convert.ToInt32(read["isApprovedDM"]);
                    o.isApprovedAM = Convert.ToInt32(read["isApprovedAM"]);
                    o.isApprovedRM = Convert.ToInt32(read["isApprovedRM"]);
                    o.isApprovedPres = Convert.ToInt32(read["isApprovedPres"].ToString().Trim());

                    o.reqDM = Convert.ToInt32(read["isDMApproval"]);
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

                    if (o.isDivRequest == "1")
                    {
                        o.BranchName = oReq.getBranchname(o.BranchCode, "HO", o.ZoneCode);
                    }
                    else
                    {
                        o.BranchName = toTC.ToTitleCase(oReq.getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
                    }
                    data.Add(o);
                }
                Info._OpenInfo = data;
            }
            return Info;
        }

        public OpenReqInfo Pending_MMD_Luzon(ORSession ss, OpenReqInfo Info)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var cmd = conn.CreateCommand();
                var data = new List<OpenReqViewModel>();             
                string MMD = " SELECT " +
                             " a.reqNumber,  a.reqCreator , a.reqDescription, COUNT(d.itemDescription) AS TotalCount, " +
                             " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                             " a.reqDate, a.TypeID , a.reqTotal, a.BranchCode, a.Region, a.DivCode, a.Zonecode, a.reqStatus," +
                             " a.isDivRequest, a.DeptCode, a.forPresident," +
                             " IF(b.isApprovedDM = c.isDMApproval, 1,0) AS DM ," +
                             " IF(b.isApprovedAM = c.isAMApproval,1,0) AS AM, " +
                             " IF(b.isApprovedRM = c.isRMApproval, 1, 0) AS RM, " +
                             " IF(b.isApprovedGM = c.isGMApproval, 1, 0) AS GM, b.isApprovedVPAssistant," +
                             " IF(b.isApprovedDiv1 = c.isDivManApproval, 1, 0) AS Div1, " +
                             " IF(b.isApprovedDiv2 = c.isDivManApproval2, 1, 0) AS Div2, " +
                             " IF(b.isApprovedDiv3 = c.isDivManApproval3, 1, 0) AS Div3, " +
                             " IF(b.isApprovedPres = c.isPresidentApproval, 1, 0) AS President, " +
                             " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit, b.isApprovedDiv1, " +
                             " b.isApprovedDM, c.isDMApproval, b.isApprovedLocalDiv, b.isApprovedAM, c.isAMApproval, b.isApprovedRM, c.isRMApproval, " +
                             " b.isApprovedPres, c.isDivManApproval, b.isApprovedDiv2, c.isDivManApproval2 ,b.isApprovedDiv3, c.isDivManApproval3, " +
                             " c.DivCode1, c.DivCode2, c.DivCode3 " +
                             " FROM onlineRequest_Open a " +
                             " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                             " INNER JOIN requestType c ON a.TypeID = c.TypeID " +
                             " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber  " +
                             " WHERE a.TypeID = c.TypeID AND a.reqStatus = 'PENDING' AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                             " GROUP BY a.reqNumber ORDER BY a.syscreated ASC";
                cmd.CommandText = MMD;
                cmd.Parameters.AddWithValue("@Divcode", ss.s_DivCode);
                conn.Open();
                using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (read.Read())
                    {
                        var o = new OpenReqViewModel();
                        o.reqNumber = read["reqNumber"].ToString().Trim();
                        o.reqCreator = read["reqCreator"].ToString().Trim();
                        o.reqDescription = read["reqDescription"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(read["reqDate"].ToString()));
                        o.TypeID = read["TypeID"].ToString().Trim();

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

                        o.TotalItems = read["TotalCount"].ToString().Trim();
                        o.itemDescription = read["Description"].ToString().Trim();
                        o.BranchCode = read["BranchCode"].ToString().Trim();
                        o.ZoneCode = read["Zonecode"].ToString().Trim();
                        o.isDivRequest = read["isDivRequest"].ToString().Trim();
                        o.DeptCode = read["DeptCode"].ToString().Trim();
                        o.Region = read["Region"].ToString().Trim();
                        if (o.Region == "HO")
                        {
                            o.Region = oReq.GetOR_DivisionName(o.DeptCode).ToUpper();
                        }
                        o.forPresident = read["forPresident"].ToString().Trim();
                        if (!(new[] { "001", "002" }).Contains(o.BranchCode))
                        {
                            o.result_AM = Convert.ToInt32(read["AM"].ToString().Trim());
                            o.result_RM = Convert.ToInt32(read["RM"].ToString().Trim());
                        }

                        o.result_DM = Convert.ToInt32(read["DM"]);
                        o.isApprovedLocalDiv = Convert.ToInt32(read["isApprovedLocalDiv"]);

                        o.result_Div1 = Convert.ToInt32(read["Div1"]);
                        o.result_Div2 = Convert.ToInt32(read["Div2"]);
                        o.result_Div3 = Convert.ToInt32(read["Div3"]);
                        o.result_GM = Convert.ToInt32(read["GM"]);
                        o.result_Pres = Convert.ToInt32(read["President"]);

                        o.MMD_Processed = Convert.ToInt32(read["isMMDProcessed"]);
                        o.MMD_ForDelivery = Convert.ToInt32(read["isDelivered"]);
                        o.MMD_InTransit = Convert.ToInt32(read["isMMDTransit"]);

                        o.isApprovedDM = Convert.ToInt32(read["isApprovedDM"]);
                        o.isApprovedAM = Convert.ToInt32(read["isApprovedAM"]);
                        o.isApprovedRM = Convert.ToInt32(read["isApprovedRM"]);
                        o.isApprovedPres = Convert.ToInt32(read["isApprovedPres"].ToString().Trim());

                        o.reqDM = Convert.ToInt32(read["isDMApproval"]);
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

                        if (o.isDivRequest == "1")
                        {
                            o.BranchName = oReq.getBranchname(o.BranchCode, "HO", o.ZoneCode);
                        }
                        else
                        {
                            o.BranchName = toTC.ToTitleCase(oReq.getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower());
                        }
                        data.Add(o);
                    }
                }
                Info._OpenInfo = data;
            }
            return Info;
        }

        public OpenReqInfo Pending_AM(ORSession ss, OpenReqInfo Info)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var cmd = conn.CreateCommand();
                var data = new List<OpenReqViewModel>();

                cmd.CommandText = " SELECT " +
                                  " a.reqNumber,  a.reqCreator , a.reqDescription, COUNT(d.itemDescription) AS TotalCount,  " +
                                  " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqDate, a.TypeID , a.reqTotal, a.BranchCode, a.Region, a.DivCode, a.Zonecode, a.reqStatus, a.isDivRequest, b.isApprovedLocalDiv, a.DeptCode, a.forPresident, " +
                                  " b.isApprovedDM, c.isDMApproval," +
                                  " b.isApprovedAM, c.isAMApproval, " +
                                  " b.isApprovedRM ,c.isRMApproval, " +
                                  " b.isApprovedGM, c.isGMApproval, b.isApprovedVPAssistant," +
                                  " b.isApprovedDiv1, c.isDivManApproval, " +
                                  " b.isApprovedDiv2, c.isDivManApproval2, " +
                                  " b.isApprovedDiv3, c.isDivManApproval3, " +
                                  " b.isApprovedPres, c.isPresidentApproval, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit, c.DivCode1, c.DivCode2, c.DivCode3" +
                                  " FROM onlineRequest_Open a " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                  " INNER JOIN requestType c ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber  " +
                                  " WHERE a.reqStatus = 'PENDING' AND (c.isAMApproval = 1 AND a.Area = @Area AND a.ZoneCode = @ZoneCode)  " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                cmd.Parameters.AddWithValue("@Area", ss.s_area);
                cmd.Parameters.AddWithValue("@ZoneCode", ss.s_zonecode);
                conn.Open();
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        var o = new OpenReqViewModel();
                        o.reqNumber = rdr["reqNumber"].ToString().Trim();
                        o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                        o.reqDescription = rdr["reqDescription"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                        o.TypeID = rdr["TypeID"].ToString().Trim();

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

                        o.TotalItems = rdr["TotalCount"].ToString().Trim();
                        o.itemDescription = rdr["Description"].ToString().Trim();
                        o.BranchCode = rdr["BranchCode"].ToString().Trim();
                        o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                        o.DeptCode = rdr["DeptCode"].ToString().Trim();
                        o.Region = rdr["Region"].ToString().Trim().ToUpper();
                        if (o.Region == "HO")
                        {
                            o.Region = oReq.GetOR_DivisionName(o.DeptCode).ToUpper();
                        }
                        o.forPresident = rdr["forPresident"].ToString().Trim();
                        if (o.isDivRequest == "1")
                        {
                            o.BranchName = oReq.getBranchname(o.BranchCode, "HO", o.ZoneCode);
                        }
                        else
                        {
                            o.BranchName = toTC.ToTitleCase(oReq.getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
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

                        data.Add(o);
                    }
                }
                Info._OpenInfo = data;
                return Info;
            }
        }

        public OpenReqInfo Pending_RM(ORSession ss, OpenReqInfo Info)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var cmd = conn.CreateCommand();
                var data = new List<OpenReqViewModel>();

                cmd.CommandText = " SELECT " +
                                  " a.reqNumber,  a.reqCreator , a.reqDescription, COUNT(d.itemDescription) AS TotalCount, a.reqDate, a.TypeID , a.reqTotal, a.BranchCode, a.Region, " +
                                  " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.DivCode, a.Zonecode, a.reqStatus, a.isDivRequest, b.isApprovedLocalDiv, a.DeptCode, a.forPresident," +
                                  " b.isApprovedDM, c.isDMApproval, " +
                                  " b.isApprovedAM, c.isAMApproval, " +
                                  " b.isApprovedRM ,c.isRMApproval, " +
                                  " b.isApprovedGM, c.isGMApproval, b.isApprovedVPAssistant, " +
                                  " b.isApprovedDiv1, c.isDivManApproval, " +
                                  " b.isApprovedDiv2, c.isDivManApproval2, " +
                                  " b.isApprovedDiv3, c.isDivManApproval3, " +
                                  " b.isApprovedPres, c.isPresidentApproval, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit, c.DivCode1, c.DivCode2, c.DivCode3 " +
                                  " FROM onlineRequest_Open a " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                  " INNER JOIN requestType c ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber  " +
                                  " WHERE a.reqStatus = 'PENDING' AND (a.Region = @Region AND a.ZoneCode = @ZoneCode) " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                cmd.Parameters.AddWithValue("@Region", ss.s_region);
                cmd.Parameters.AddWithValue("@ZoneCode", ss.s_zonecode);
                conn.Open();
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        var o = new OpenReqViewModel();
                        o.reqNumber = rdr["reqNumber"].ToString().Trim();
                        o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                        o.reqDescription = rdr["reqDescription"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                        o.TypeID = rdr["TypeID"].ToString().Trim();

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

                        o.TotalItems = rdr["TotalCount"].ToString().Trim();
                        o.itemDescription = rdr["Description"].ToString().Trim();
                        o.BranchCode = rdr["BranchCode"].ToString().Trim();
                        o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                        o.DeptCode = rdr["DeptCode"].ToString().Trim();
                        o.Region = rdr["Region"].ToString().Trim().ToUpper();
                        if (o.Region == "HO")
                        {
                            o.Region = oReq.GetOR_DivisionName(o.DeptCode).ToUpper();
                        }
                        o.forPresident = rdr["forPresident"].ToString().Trim();
                        if (o.isDivRequest == "1")
                        {
                            o.BranchName = oReq.getBranchname(o.BranchCode, "HO", o.ZoneCode);
                        }
                        else
                        {
                            o.BranchName = toTC.ToTitleCase(oReq.getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
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

                        data.Add(o);
                    }
                }
                Info._OpenInfo = data;
                return Info;
            }
        }

        public OpenReqInfo Pending_Dept_Approver(ORSession ss, OpenReqInfo Info)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var cmd = conn.CreateCommand();
                var data = new List<OpenReqViewModel>();

                cmd.CommandText = " SELECT  a.reqNumber, a.reqCreator, a.reqDescription, COUNT(d.itemDescription) AS TotalCount, a.reqDate, a.TypeID , a.reqTotal, a.BranchCode, a.Region, a.DivCode, a.Zonecode,   " +
                                  " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqStatus, a.isDivRequest, a.DeptCode, a.forPresident, b.isApprovedLocalDiv, b.isApprovedDM, c.isDMApproval,  b.isApprovedAM, " +
                                  " c.isAMApproval,  b.isApprovedRM, c.isRMApproval,  b.isApprovedGM,  c.isGMApproval, b.isApprovedVPAssistant, b.isApprovedDiv1, " +
                                  " c.isDivManApproval,  b.isApprovedDiv2, c.isDivManApproval2,  b.isApprovedDiv3, c.isDivManApproval3,  b.isApprovedPres, c.isPresidentApproval,  " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit, c.DivCode1,  c.DivCode2, c.DivCode3  " +
                                  " FROM onlineRequest_Open a " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                  " INNER JOIN requestType c ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber  " +
                                  " WHERE a.reqStatus = 'Pending' AND " +
                                  " a.DeptCode = @deptCode GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";        
                cmd.Parameters.AddWithValue("@deptCode", ss.s_costcenter);
                conn.Open();
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        var o = new OpenReqViewModel();
                        o.reqNumber = rdr["reqNumber"].ToString().Trim();
                        o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                        o.reqDescription = rdr["reqDescription"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                        o.TypeID = rdr["TypeID"].ToString().Trim();

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

                        o.TotalItems = rdr["TotalCount"].ToString().Trim();
                        o.itemDescription = rdr["Description"].ToString().Trim();
                        o.BranchCode = rdr["BranchCode"].ToString().Trim();
                        o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                        o.DeptCode = rdr["DeptCode"].ToString().Trim();
                        o.Region = rdr["Region"].ToString().Trim().ToUpper();

                        if (o.Region == "HO")
                        {
                            o.Region = oReq.GetOR_DivisionName(o.DeptCode).ToUpper();
                        }
                        o.forPresident = rdr["forPresident"].ToString().Trim();
                        if (o.isDivRequest == "1")
                        {
                            o.BranchName = oReq.getBranchname(o.BranchCode, "HO", o.ZoneCode);
                        }
                        else
                        {
                            o.BranchName = toTC.ToTitleCase(oReq.getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
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

                        data.Add(o);
                    }
                }
                Info._OpenInfo = data;
                return Info;
            }
        }

        public OpenReqInfo Pending_Div_Approver(ORSession ss, OpenReqInfo Info)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var cmd = conn.CreateCommand();
                var data = new List<OpenReqViewModel>();

                cmd.CommandText = " SELECT  a.reqNumber, a.reqCreator, a.reqDescription, COUNT(d.itemDescription) AS TotalCount, a.reqDate, a.TypeID , a.reqTotal, a.BranchCode, a.Region, a.DivCode, a.Zonecode,   " +
                                  " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqStatus, a.isDivRequest, a.DeptCode, a.forPresident, b.isApprovedLocalDiv, b.isApprovedDM, c.isDMApproval,  b.isApprovedAM, " +
                                  " c.isAMApproval,  b.isApprovedRM, c.isRMApproval,  b.isApprovedGM,  c.isGMApproval, b.isApprovedVPAssistant,  b.isApprovedDiv1, " +
                                  " c.isDivManApproval,  b.isApprovedDiv2, c.isDivManApproval2,  b.isApprovedDiv3, c.isDivManApproval3,  b.isApprovedPres, c.isPresidentApproval,  " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit, c.DivCode1,  c.DivCode2, c.DivCode3  " +
                                  " FROM onlineRequest_Open a " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                  " INNER JOIN requestType c ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber  " +
                                  " WHERE a.reqStatus = 'Pending' AND " +
                                  " a.DeptCode = @deptCode  GROUP BY a.reqNumber ORDER BY a.syscreated ASC  ";
                cmd.Parameters.AddWithValue("@deptCode", ss.s_costcenter);
                conn.Open();
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        var o = new OpenReqViewModel();
                        o.reqNumber = rdr["reqNumber"].ToString().Trim();
                        o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                        o.reqDescription = rdr["reqDescription"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                        o.TypeID = rdr["TypeID"].ToString().Trim();

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

                        o.TotalItems = rdr["TotalCount"].ToString().Trim();
                        o.itemDescription = rdr["Description"].ToString().Trim();
                        o.BranchCode = rdr["BranchCode"].ToString().Trim();
                        o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                        o.DeptCode = rdr["DeptCode"].ToString().Trim();
                        o.Region = rdr["Region"].ToString().Trim().ToUpper();
                        if (o.Region == "HO")
                        {
                            o.Region = oReq.GetOR_DivisionName(o.DeptCode).ToUpper();
                        }
                        o.forPresident = rdr["forPresident"].ToString().Trim();
                        if (o.isDivRequest == "1")
                        {
                            o.BranchName = oReq.getBranchname(o.BranchCode, "HO", o.ZoneCode);
                        }
                        else
                        {
                            o.BranchName = toTC.ToTitleCase(oReq.getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
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

                        data.Add(o);
                    }
                }
                Info._OpenInfo = data;
                return Info;
            }
        }

        public OpenReqInfo Pending_GMO(ORSession ss, OpenReqInfo Info)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var toTC = new CultureInfo("en-US", false).TextInfo;              
                var data = new List<OpenReqViewModel>();

                var cmd = conn.CreateCommand();
                cmd.CommandText = " SELECT  a.reqNumber, a.reqCreator, a.reqDescription, COUNT(d.itemDescription) AS TotalCount, a.reqDate, a.TypeID , a.reqTotal, a.BranchCode, a.Region, a.DivCode, a.Zonecode,   " +
                                  " ( SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqStatus, a.isDivRequest, a.DeptCode, a.forPresident, b.isApprovedLocalDiv, b.isApprovedDM, c.isDMApproval,  b.isApprovedAM, " +
                                  " c.isAMApproval,  b.isApprovedRM, c.isRMApproval,  b.isApprovedGM,  c.isGMApproval, b.isApprovedVPAssistant, b.isApprovedDiv1, " +
                                  " c.isDivManApproval,  b.isApprovedDiv2, c.isDivManApproval2,  b.isApprovedDiv3, c.isDivManApproval3,  b.isApprovedPres, c.isPresidentApproval,  " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit, c.DivCode1,  c.DivCode2, c.DivCode3  " +
                                  " FROM onlineRequest_Open a " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                  " INNER JOIN requestType c ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber  " +
                                  " WHERE a.reqStatus = 'PENDING' AND " +
                                  " a.DeptCode = @deptCode GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                cmd.Parameters.AddWithValue("@deptCode", ss.s_costcenter);
                conn.Open();
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        var o = new OpenReqViewModel();
                        o.reqNumber = rdr["reqNumber"].ToString().Trim();
                        o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                        o.reqDescription = rdr["reqDescription"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                        o.TypeID = rdr["TypeID"].ToString().Trim();

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

                        o.TotalItems = rdr["TotalCount"].ToString().Trim();
                        o.itemDescription = rdr["Description"].ToString().Trim();
                        o.BranchCode = rdr["BranchCode"].ToString().Trim();
                        o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                        o.DeptCode = rdr["DeptCode"].ToString().Trim();
                        o.Region = rdr["Region"].ToString().Trim().ToUpper();
                        if (o.Region == "HO")
                        {
                            o.Region = oReq.GetOR_DivisionName(o.DeptCode).ToUpper();
                        }
                        o.forPresident = rdr["forPresident"].ToString().Trim();
                        if (o.isDivRequest == "1")
                        {
                            o.BranchName = oReq.getBranchname(o.BranchCode, "HO", o.ZoneCode);
                        }
                        else
                        {
                            o.BranchName = toTC.ToTitleCase(oReq.getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
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

                        data.Add(o);
                    }
                }
                Info._OpenInfo = data;
                return Info;
            }
        }

        public OpenReqInfo Pending_President(ORSession ss, OpenReqInfo Info)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var cmd = conn.CreateCommand();
                var data = new List<OpenReqViewModel>();

                cmd.CommandText = " SELECT  a.reqNumber, a.reqCreator, a.reqDescription, COUNT(d.itemDescription) AS TotalCount, a.reqDate, a.TypeID , a.reqTotal, a.BranchCode, a.Region, a.DivCode, a.Zonecode,   " +
                                  " ( SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqStatus, a.isDivRequest, a.DeptCode, a.forPresident, b.isApprovedLocalDiv, b.isApprovedDM, c.isDMApproval,  b.isApprovedAM, " +
                                  " c.isAMApproval,  b.isApprovedRM, c.isRMApproval,  b.isApprovedGM,  c.isGMApproval, b.isApprovedVPAssistant, b.isApprovedDiv1, " +
                                  " c.isDivManApproval,  b.isApprovedDiv2, c.isDivManApproval2,  b.isApprovedDiv3, c.isDivManApproval3,  b.isApprovedPres, c.isPresidentApproval,  " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit, c.DivCode1,  c.DivCode2, c.DivCode3  " +
                                  " FROM onlineRequest_Open a " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                  " INNER JOIN requestType c ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber  " +
                                  " WHERE a.reqStatus = 'Pending' AND " +
                                  " a.DeptCode = @deptCode GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                cmd.Parameters.AddWithValue("@deptCode", ss.s_costcenter);
                conn.Open();
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        var o = new OpenReqViewModel();
                        o.reqNumber = rdr["reqNumber"].ToString().Trim();
                        o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                        o.reqDescription = rdr["reqDescription"].ToString().Trim();
                        o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                        o.TypeID = rdr["TypeID"].ToString().Trim();

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

                        o.TotalItems = rdr["TotalCount"].ToString().Trim();
                        o.itemDescription = rdr["Description"].ToString().Trim();

                        o.BranchCode = rdr["BranchCode"].ToString().Trim();
                        o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                        o.DeptCode = rdr["DeptCode"].ToString().Trim();
                        o.Region = rdr["Region"].ToString().Trim().ToUpper();
                        if (o.Region == "HO")
                        {
                            o.Region = oReq.GetOR_DivisionName(o.DeptCode).ToUpper();
                        }
                        o.forPresident = rdr["forPresident"].ToString().Trim();
                        if (o.isDivRequest == "1")
                        {
                            o.BranchName = oReq.getBranchname(o.BranchCode, "HO", o.ZoneCode);
                        }
                        else
                        {
                            o.BranchName = toTC.ToTitleCase(oReq.getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
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

                        data.Add(o);
                    }
                }
                Info._OpenInfo = data;
                return Info;
            }
        }
    }
}