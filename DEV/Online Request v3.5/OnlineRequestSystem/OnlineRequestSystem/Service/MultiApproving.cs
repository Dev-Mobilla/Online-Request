using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OnlineRequestSystem.Controllers;
using OnlineRequestSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace OnlineRequestSystem.Service
{
    public class MultiApproving
    {
        private Helper que = new Helper();
        private DisapproveReqController disp = new DisapproveReqController();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MultiApproving));
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;
        private string format = "yyyy/MM/dd HH:mm:ss";

        internal string ApproveAM(string ReqNo, ORSession ss)
        {
            var objects = JsonConvert.DeserializeObject<List<object>>(ReqNo);
            var db = new ORtoMySql();

            try
            {
                using (var conn = db.getConnection())
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();

                    for (int i = 0; i < objects.Count; i++)
                    {
                        string RequestNo = objects[i].ToString();
                        string UpdateAM = " UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationAM_Name = @E_AMname, EscalationAM_Date = @E_AMDate, EscalationAM_Remarks = @E_AMRemarks WHERE reqNumber = @ReqNo ";
                        cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                        cmd.Parameters.AddWithValue("@E_AMName", ss.s_fullname);
                        cmd.Parameters.AddWithValue("@E_AMDate", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@E_AMRemarks", "Approved by Area Manager.");
                        cmd.CommandText = UpdateAM;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        string Stat_AM = " UPDATE OnlineRequest.requestApproverStatus SET AM_Approver = @Approver , AM_Approved_Date = @Date, isApprovedAM = @isApproved WHERE reqNumber = @ReqNo";
                        cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                        cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                        cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@isApproved", 1);
                        cmd.CommandText = Stat_AM;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        que.SetDateModified(RequestNo, ss.s_usr_id);
                        log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " .  [ Area Manager ]");
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return "error";
            }
            return "success";
        }

        internal string DisapproveAM()
        {
            return "success";
        }

        internal string ApproveRM(string ReqNo, ORSession ss)
        {
            try
            {
                var objects = JsonConvert.DeserializeObject<List<object>>(ReqNo);
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();

                    for (int i = 0; i < objects.Count; i++)
                    {
                        string RequestNo = objects[i].ToString();
                        string UpdateAM = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationRM_Name = @approver, EscalationRM_Date = @datenow, EscalationRM_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                        cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                        cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                        cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@remarks", "Approved by Regional Manager.");
                        cmd.CommandText = UpdateAM;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        string Stat_AM = "UPDATE OnlineRequest.requestApproverStatus SET RM_Approver = @Approver, RM_Approved_Date = @Date, isApprovedRM = @isApproved WHERE reqNumber = @ReqNo";
                        cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                        cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                        cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@isApproved", 1);
                        cmd.CommandText = Stat_AM;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        que.SetDateModified(RequestNo, ss.s_usr_id);
                        log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " .  [ Regional Manager ]");
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return "error";
            }
            return "success";
        }

        internal string ApproveDM(string ReqNo, ORSession ss)
        {
            try
            {
                var objects = JsonConvert.DeserializeObject<List<object>>(ReqNo);
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();

                    for (int i = 0; i < objects.Count; i++)
                    {
                        string RequestNo = objects[i].ToString();
                        string UpdateAM = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDM_Name = @approver, EscalationDM_Date = @datenow, EscalationDM_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                        cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                        cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                        cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@remarks", "Approved by Department Manager.");
                        cmd.CommandText = UpdateAM;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        string Stat_AM = "UPDATE OnlineRequest.requestApproverStatus SET DM_Approver = @Approver, DM_Approved_Date = @Date, isApprovedDM = @isApproved WHERE reqNumber = @ReqNo";
                        cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                        cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                        cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@isApproved", 1);
                        cmd.CommandText = Stat_AM;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        que.SetDateModified(RequestNo, ss.s_usr_id);
                        log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Department Approver ]");
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return "error";
            }
            return "success";
        }

        internal string ApproveDivApprover(string ReqNo, ORSession ss)
        {
            try
            {
                var objects = JsonConvert.DeserializeObject<List<object>>(ReqNo);
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    for (int i = 0; i < objects.Count; i++)
                    {
                        string RequestNo = objects[i].ToString();
                        object[] data = GetOpenRequestData(RequestNo);
                        if (data[6].ToString() == "0" && data[0].ToString() == ss.s_costcenter)
                        {    
                            #region Update Local Division Approver

                            string UpdateLocalDiv = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationLocalDiv_Name = @approver, EscalationLocalDiv_Date = @datenow, EscalationLocalDiv_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                            cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                            cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                            cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@remarks", "Approved by Division Manager.");
                            cmd.CommandText = UpdateLocalDiv;
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            string Stat_LocalDiv = "UPDATE OnlineRequest.requestApproverStatus SET LocalDiv_Approver = @Approver, LocalDiv_Approved_Date = @Date, isApprovedLocalDiv = @isApproved WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                            cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                            cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@isApproved", 1);
                            cmd.CommandText = Stat_LocalDiv;
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            que.SetDateModified(RequestNo, ss.s_usr_id);
                            log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Local Division ]");

                            #endregion Update Local Division Approver

                            if (data[2].ToString() == ss.s_DivisionID)
                            {
                                #region Update Division Approver 1

                                string UpdateDiv1 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv_Name = @approver, EscalationDiv_Date = @datenow, EscalationDiv_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved by Division Manager.");
                                cmd.CommandText = UpdateDiv1;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div1 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver1 = @Approver, DivCode1 = @divCode,  Div_Approved_Date1 = @Date, isApprovedDiv1 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div1;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " .  [ Division Approver 1 ]");

                                #endregion Update Division Approver 1
                            }
                            if (data[3].ToString() == ss.s_DivisionID)
                            { 
                                #region Update Division Approver 2

                                string UpdateDiv2 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv2_Name = @approver, EscalationDiv2_Date = @datenow, EscalationDiv2_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved by Division Manager.");
                                cmd.CommandText = UpdateDiv2;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div2 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver2 = @Approver, DivCode2 = @divCode,  Div_Approved_Date2 = @Date, isApprovedDiv2 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div2;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 2 ]");

                                #endregion Update Division Approver 2
                            }
                            if (data[4].ToString() == ss.s_DivisionID)
                            { 
                                #region Update Division Approver 3

                                string UpdateDiv3 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv3_Name = @approver, EscalationDiv3_Date = @datenow, EscalationDiv3_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved by Division Manager.");
                                cmd.CommandText = UpdateDiv3;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div3 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver3 = @Approver, DivCode3 = @divCode,  Div_Approved_Date3 = @Date, isApprovedDiv3 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div3;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 3 ]");

                                #endregion Update Division Approver 3
                            }
                        }
                        else
                        {
                            if (data[2].ToString() == ss.s_DivisionID)
                            { 
                                #region Update Division Approver 1

                                string UpdateDiv1 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv_Name = @approver, EscalationDiv_Date = @datenow, EscalationDiv_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                cmd.CommandText = UpdateDiv1;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div1 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver1 = @Approver, DivCode1 = @divCode,  Div_Approved_Date1 = @Date, isApprovedDiv1 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div1;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 1 ]");

                                #endregion Update Division Approver 1
                            }
                            if (data[3].ToString() == ss.s_DivisionID)
                            { 
                                #region Update Division Approver 2

                                string UpdateDiv2 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv2_Name = @approver, EscalationDiv2_Date = @datenow, EscalationDiv2_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                cmd.CommandText = UpdateDiv2;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div2 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver2 = @Approver, DivCode2 = @divCode,  Div_Approved_Date2 = @Date, isApprovedDiv2 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div2;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 2 ]");

                                #endregion Update Division Approver 2
                            }
                            if (data[4].ToString() == ss.s_DivisionID)
                            { 
                                #region Update Division Approver 3

                                string UpdateDiv3 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv3_Name = @approver, EscalationDiv3_Date = @datenow, EscalationDiv3_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                cmd.CommandText = UpdateDiv3;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div3 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver3 = @Approver, DivCode3 = @divCode,  Div_Approved_Date3 = @Date, isApprovedDiv3 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div3;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 3 ]");

                                #endregion Update Division Approver 3
                            }
                        }
                    }
                }
                return "success";
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return x.ToString();
            }
        }

        private object[] GetOpenRequestData(string RequestNo)
        {
            try
            {
                object[] data = new object[11];
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        String x = " SELECT a.reqNumber, a.reqCreator, a.TypeID, a.DeptCode, a.isDivRequest, " +
                                   " b.DivCode1, b.DivCode2, b.DivCode3,c.isApprovedDM, c.isApprovedLocalDiv, c.isApprovedDiv1, c.isApprovedDiv2, c.isApprovedDiv3 ,forPresident" +
                                   " FROM onlineRequest_Open a " +
                                   " INNER JOIN requestType b ON a.TypeID = b.TypeID " +
                                   " INNER JOIN requestApproverStatus c ON c.reqNumber = a.reqNumber " +
                                   " WHERE a.reqNumber=@reqNumber;";
                        cmd.Parameters.AddWithValue("@reqNumber", RequestNo);
                        cmd.CommandText = x;
                        cmd.CommandTimeout = 0;
                        using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (read.HasRows)
                            {
                                #region Retrieve data from database

                                read.Read();
                                data[0] = read["DeptCode"].ToString().Trim();
                                data[1] = read["isDivRequest"].ToString().Trim();
                                data[2] = read["DivCode1"].ToString().Trim();
                                data[3] = read["DivCode2"].ToString().Trim();
                                data[4] = read["DivCode3"].ToString().Trim();
                                data[5] = read["isApprovedDM"].ToString().Trim();
                                data[6] = read["isApprovedLocalDiv"].ToString().Trim();
                                data[7] = read["isApprovedDiv1"].ToString().Trim();
                                data[8] = read["isApprovedDiv2"].ToString().Trim();
                                data[9] = read["isApprovedDiv3"].ToString().Trim();
                                data[10] = read["forPresident"].ToString().Trim();

                                #endregion Retrieve data from database
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        internal string ApproveVPAssistant(string ReqNo, ORSession ss)
        {
            try
            {
                var objects = JsonConvert.DeserializeObject<List<object>>(ReqNo);
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();

                    for (int i = 0; i < objects.Count; i++)
                    {
                        #region update Escalation & Req Approver status

                        string RequestNo = objects[i].ToString();
                        cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                        cmd.Parameters.AddWithValue("@EApprover", ss.s_fullname);
                        cmd.Parameters.AddWithValue("@EDatenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@ERemarks", "Approved by VP Assistant.");
                        cmd.CommandText = " UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationVPAssistant = @EApprover, EscalationVPAssistant_Date = @EDatenow, EscalationVPAssistant_Remarks = @ERemarks WHERE reqNumber = @ReqNo "; ;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                  
                        cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                        cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                        cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@isApproved", 1);
                        cmd.CommandText = " UPDATE OnlineRequest.requestApproverStatus SET VPAssistant_Approver = @Approver, VPAssistant_Approved_Date = @Date, isApprovedVPAssistant = @isApproved WHERE reqNumber = @ReqNo"; ;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        #endregion update Escalation & Req Approver status

                        que.SetDateModified(RequestNo, ss.s_usr_id);
                        log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ VP Assistant ]");
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return "error";
            }
            return "success";
        }

        internal string ApproveVP(string ReqNo, ORSession ss)
        {
            try
            {
                var objects = JsonConvert.DeserializeObject<List<object>>(ReqNo);
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    for (int i = 0; i < objects.Count; i++)
                    {
                        string RequestNo = objects[i].ToString();
                        object[] data = GetOpenRequestData(RequestNo);
                        if (data[1].ToString() == "1")
                        {
                            if (data[6].ToString() == "0" && data[0].ToString() == ss.s_costcenter)
                            {  
                                #region Update VP

                                string VP = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationGM_Name = @approver, EscalationGM_Date = @datenow, EscalationGM_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved by Vice President.");
                                cmd.CommandText = VP;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_VP = "UPDATE OnlineRequest.requestApproverStatus SET GM_Approver = @Approver, GM_Approved_Date = @Date, isApprovedGM = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_VP;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + ". [ Vice President ]");

                                #endregion Update VP

                                if (data[2].ToString() == ss.s_DivisionID)
                                { 
                                    #region Update Division Approver 1

                                    string UpdateDiv1 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv_Name = @approver, EscalationDiv_Date = @datenow, EscalationDiv_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                    cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                    cmd.CommandText = UpdateDiv1;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();

                                    string Stat_Div1 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver1 = @Approver, DivCode1 = @divCode,  Div_Approved_Date1 = @Date, isApprovedDiv1 = @isApproved WHERE reqNumber = @ReqNo";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                    cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                    cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@isApproved", 1);
                                    cmd.CommandText = Stat_Div1;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    que.SetDateModified(RequestNo, ss.s_usr_id);
                                    log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 1 ]");

                                    #endregion Update Division Approver 1
                                }
                                if (data[3].ToString() == ss.s_DivisionID)
                                {
                                    #region Update Division Approver 2

                                    string UpdateDiv2 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv2_Name = @approver, EscalationDiv2_Date = @datenow, EscalationDiv2_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                    cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                    cmd.CommandText = UpdateDiv2;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();

                                    string Stat_Div2 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver2 = @Approver, DivCode2 = @divCode,  Div_Approved_Date2 = @Date, isApprovedDiv2 = @isApproved WHERE reqNumber = @ReqNo";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                    cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                    cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@isApproved", 1);
                                    cmd.CommandText = Stat_Div2;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    que.SetDateModified(RequestNo, ss.s_usr_id);
                                    log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 2 ]");

                                    #endregion Update Division Approver 2
                                }
                                if (data[4].ToString() == ss.s_DivisionID)
                                { 
                                    #region Update Division Approver 3

                                    string UpdateDiv3 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv3_Name = @approver, EscalationDiv3_Date = @datenow, EscalationDiv3_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                    cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                    cmd.CommandText = UpdateDiv3;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();

                                    string Stat_Div3 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver3 = @Approver, DivCode3 = @divCode,  Div_Approved_Date3 = @Date, isApprovedDiv3 = @isApproved WHERE reqNumber = @ReqNo";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                    cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                    cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@isApproved", 1);
                                    cmd.CommandText = Stat_Div3;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    que.SetDateModified(RequestNo, ss.s_usr_id);
                                    log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 3 ]");

                                    #endregion Update Division Approver 3
                                }
                            }
                        }
                        else
                        {
                            #region Update VP Approval

                            string VP = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationGM_Name = @approver, EscalationGM_Date = @datenow, EscalationGM_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                            cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                            cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                            cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@remarks", "Approved by Vice President.");
                            cmd.CommandText = VP;
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            string Stat_VP = "UPDATE OnlineRequest.requestApproverStatus SET GM_Approver = @Approver, GM_Approved_Date = @Date, isApprovedGM = @isApproved WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                            cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                            cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@isApproved", 1);
                            cmd.CommandText = Stat_VP;
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            que.SetDateModified(RequestNo, ss.s_usr_id);
                            log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Vice President ]");

                            #endregion Update VP Approval

                            if (data[2].ToString() == ss.s_DivisionID)
                            { 
                                #region Update Division Approver 1

                                string UpdateDiv1 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv_Name = @approver, EscalationDiv_Date = @datenow, EscalationDiv_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                cmd.CommandText = UpdateDiv1;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div1 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver1 = @Approver, DivCode1 = @divCode,  Div_Approved_Date1 = @Date, isApprovedDiv1 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div1;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 1 ]");

                                #endregion Update Division Approver 1
                            }
                            if (data[3].ToString() == ss.s_DivisionID)
                            { 
                                #region Update Division Approver 2

                                string UpdateDiv2 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv2_Name = @approver, EscalationDiv2_Date = @datenow, EscalationDiv2_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                cmd.CommandText = UpdateDiv2;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div2 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver2 = @Approver, DivCode2 = @divCode,  Div_Approved_Date2 = @Date, isApprovedDiv2 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div2;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 2 ]");

                                #endregion Update Division Approver 2
                            }
                            if (data[4].ToString() == ss.s_DivisionID)
                            { 
                                #region Update Division Approver 3

                                string UpdateDiv3 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv3_Name = @approver, EscalationDiv3_Date = @datenow, EscalationDiv3_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                cmd.CommandText = UpdateDiv3;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div3 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver3 = @Approver, DivCode3 = @divCode,  Div_Approved_Date3 = @Date, isApprovedDiv3 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div3;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 3 ]");

                                #endregion Update Division Approver 3
                            }
                        }
                    }
                }
                return "success";
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return x.ToString();
            }
        }

        internal string ApprovedPresident(string ReqNo, ORSession ss)
        {
            try
            {
                var objects = JsonConvert.DeserializeObject<List<object>>(ReqNo);
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    for (int i = 0; i < objects.Count; i++)
                    {
                        string RequestNo = objects[i].ToString();
                        object[] data = GetOpenRequestData(RequestNo);
                        if (data[1].ToString() == "1")
                        {
                            if (data[10].ToString() == "1")
                            {
                                #region Update President Approval

                                string UpdatePresident = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationPres_Name = @approver, EscalationPres_Date = @datenow, EscalationPres_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved by President.");
                                cmd.CommandText = UpdatePresident;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_President = "UPDATE OnlineRequest.requestApproverStatus SET Pres_Approver = @Approver, Pres_Approved_Date = @Date, isApprovedPres = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_President;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ President ]");

                                #endregion Update President Approval
                            }
                            if (data[6].ToString() == "0" && data[0].ToString() == ss.s_costcenter)
                            {  
                                #region Update Local Division Approver

                                string UpdateLocalDiv = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationLocalDiv_Name = @approver, EscalationLocalDiv_Date = @datenow, EscalationLocalDiv_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved by Vice President.");
                                cmd.CommandText = UpdateLocalDiv;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_LocalDiv = "UPDATE OnlineRequest.requestApproverStatus SET LocalDiv_Approver = @Approver, LocalDiv_Approved_Date = @Date, isApprovedLocalDiv = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_LocalDiv;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Local Division ]");

                                #endregion Update Local Division Approver

                                if (data[2].ToString() == ss.s_DivisionID)
                                { 
                                    #region Update Division Approver 1

                                    string UpdateDiv1 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv_Name = @approver, EscalationDiv_Date = @datenow, EscalationDiv_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                    cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                    cmd.CommandText = UpdateDiv1;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();

                                    string Stat_Div1 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver1 = @Approver, DivCode1 = @divCode,  Div_Approved_Date1 = @Date, isApprovedDiv1 = @isApproved WHERE reqNumber = @ReqNo";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                    cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                    cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@isApproved", 1);
                                    cmd.CommandText = Stat_Div1;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    que.SetDateModified(RequestNo, ss.s_usr_id);
                                    log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 1 ]");

                                    #endregion Update Division Approver 1
                                }
                                if (data[3].ToString() == ss.s_DivisionID)
                                {
                                    #region Update Division Approver 2

                                    string UpdateDiv2 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv2_Name = @approver, EscalationDiv2_Date = @datenow, EscalationDiv2_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                    cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                    cmd.CommandText = UpdateDiv2;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();

                                    string Stat_Div2 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver2 = @Approver, DivCode2 = @divCode,  Div_Approved_Date2 = @Date, isApprovedDiv2 = @isApproved WHERE reqNumber = @ReqNo";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                    cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                    cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@isApproved", 1);
                                    cmd.CommandText = Stat_Div2;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    que.SetDateModified(RequestNo, ss.s_usr_id);
                                    log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 2 ]");

                                    #endregion Update Division Approver 2
                                }
                                if (data[4].ToString() == ss.s_DivisionID)
                                { 
                                    #region Update Division Approver 3

                                    string UpdateDiv3 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv3_Name = @approver, EscalationDiv3_Date = @datenow, EscalationDiv3_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                    cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                    cmd.CommandText = UpdateDiv3;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();

                                    string Stat_Div3 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver3 = @Approver, DivCode3 = @divCode,  Div_Approved_Date3 = @Date, isApprovedDiv3 = @isApproved WHERE reqNumber = @ReqNo";
                                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                    cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                    cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                    cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                    cmd.Parameters.AddWithValue("@isApproved", 1);
                                    cmd.CommandText = Stat_Div3;
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    que.SetDateModified(RequestNo, ss.s_usr_id);
                                    log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 3 ]");

                                    #endregion Update Division Approver 3
                                }
                            }
                        }
                        else
                        {
                            #region Update President Approval

                            string UpdatePresident = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationPres_Name = @approver, EscalationPres_Date = @datenow, EscalationPres_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                            cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                            cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                            cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@remarks", "Approved by President.");
                            cmd.CommandText = UpdatePresident;
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            string Stat_President = "UPDATE OnlineRequest.requestApproverStatus SET Pres_Approver = @Approver, Pres_Approved_Date = @Date, isApprovedPres = @isApproved WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                            cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                            cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@isApproved", 1);
                            cmd.CommandText = Stat_President;
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            que.SetDateModified(RequestNo, ss.s_usr_id);
                            log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ President ]");

                            #endregion Update President Approval

                            if (data[2].ToString() == ss.s_DivisionID)
                            { 
                                #region Update Division Approver 1

                                string UpdateDiv1 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv_Name = @approver, EscalationDiv_Date = @datenow, EscalationDiv_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                cmd.CommandText = UpdateDiv1;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div1 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver1 = @Approver, DivCode1 = @divCode,  Div_Approved_Date1 = @Date, isApprovedDiv1 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div1;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 1 ]");

                                #endregion Update Division Approver 1
                            }
                            if (data[3].ToString() == ss.s_DivisionID)
                            { 
                                #region Update Division Approver 2

                                string UpdateDiv2 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv2_Name = @approver, EscalationDiv2_Date = @datenow, EscalationDiv2_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                cmd.CommandText = UpdateDiv2;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div2 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver2 = @Approver, DivCode2 = @divCode,  Div_Approved_Date2 = @Date, isApprovedDiv2 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div2;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 2 ]");

                                #endregion Update Division Approver 2
                            }
                            if (data[4].ToString() == ss.s_DivisionID)
                            {
                                #region Update Division Approver 3

                                string UpdateDiv3 = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv3_Name = @approver, EscalationDiv3_Date = @datenow, EscalationDiv3_Remarks = @remarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@approver", ss.s_fullname);
                                cmd.Parameters.AddWithValue("@datenow", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@remarks", "Approved.");
                                cmd.CommandText = UpdateDiv3;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                string Stat_Div3 = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver3 = @Approver, DivCode3 = @divCode,  Div_Approved_Date3 = @Date, isApprovedDiv3 = @isApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                                cmd.Parameters.AddWithValue("@Approver", ss.s_usr_id);
                                cmd.Parameters.AddWithValue("@divCode", ss.s_DivCode);
                                cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@isApproved", 1);
                                cmd.CommandText = Stat_Div3;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                que.SetDateModified(RequestNo, ss.s_usr_id);
                                log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been approved by : " + ss.s_fullname + " . [ Division Approver 3 ]");

                                #endregion Update Division Approver 3
                            }
                        }
                    }
                }
                return "success";
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return x.ToString();
            }
        }

        internal string MultipleDisapprove(string ReqNo, ORSession ss, string approver)
        {
            try
            {
                var objects = JsonConvert.DeserializeObject<List<object>>(ReqNo);
                var db = new ORtoMySql();
                var mod = new CloseRequest();
                string newDivApprover = "";
                using (var con = db.getConnection())
                {
                    var cmd = con.CreateCommand();
                    for (int i = 0; i < objects.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        string RequestNo = objects[i].ToString();
                        if (approver == "Div")
                        {
                            newDivApprover = CheckApproverDiv(RequestNo, ss.s_DivisionID, ss.s_costcenter);
                        }
                        else
                        {
                            newDivApprover = approver;
                        }
                        string Del = "SELECT * FROM OnlineRequest.onlineRequest_Open WHERE reqNumber = @reqNo";
                        cmd.CommandText = Del;
                        cmd.Parameters.AddWithValue("@reqNo", RequestNo);
                        con.Open();
                        using (MySqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            #region Read data from DB open

                            read.Read();
                            mod.reqNumber = read["reqNumber"].ToString().Trim();
                            mod.reqDate = read["reqDate"].ToString().Trim();
                            mod.reqCreator = read["reqCreator"].ToString().Trim();
                            mod.TypeID = read["TypeID"].ToString().Trim();
                            mod.reqDescription = read["reqDescription"].ToString().Trim();
                            mod.BranchCode = read["BranchCode"].ToString().Trim();
                            mod.Area = read["Area"].ToString().Trim();
                            mod.AreaCode = read["AreaCode"].ToString().Trim();
                            mod.Region = read["Region"].ToString().Trim();
                            mod.DivCode = read["DivCode"].ToString().Trim();
                            mod.ZoneCode = read["Zonecode"].ToString().Trim();
                            mod.reqStatus = read["reqStatus"].ToString().Trim();
                            mod.isApproved = read["isApproved"].ToString().Trim();
                            mod.isDivRequest = Convert.ToInt32(read["isDivRequest"]);
                            mod.forPresident = Convert.ToInt32(read["forPresident"]);
                            mod.DeptCode = read["DeptCode"].ToString().Trim();

                            #endregion Read data from DB open
                        }
                        con.Close();
                        cmd.Parameters.Clear();

                        #region Insert to close

                        string InsertClose = " INSERT INTO OnlineRequest.onlineRequest_Close ( " +
                                             " reqNumber, reqDate, reqCreator, TypeID, reqDescription, BranchCode, Region, Area, AreaCode ,DivCode, ZoneCode, " +
                                             " reqStatus, isApproved, isDivRequest, ClosedDate, ClosedBy, Remarks, syscreated, syscreator, forPresident, deptCode ) " +
                                             " VALUES (@reqNumber, @reqDate, @reqCreator, @TypeID, @reqDescription,  @BranchCode, @Region, @Area, @AreaCode, @DivCode, @ZoneCode,  " +
                                             " @reqStatus, @isApproved, @isDivRequest, @ClosedDate, @ClosedBy, @Remarks, @syscreated, @syscreator, @forPresident, @deptCode)";
                        cmd.Parameters.AddWithValue("@reqNumber", mod.reqNumber);
                        cmd.Parameters.AddWithValue("@reqDate", Convert.ToDateTime(mod.reqDate));
                        cmd.Parameters.AddWithValue("@reqCreator", mod.reqCreator);
                        cmd.Parameters.AddWithValue("@TypeID", mod.TypeID);
                        cmd.Parameters.AddWithValue("@reqDescription", mod.reqDescription);
                        cmd.Parameters.AddWithValue("@BranchCode", mod.BranchCode);
                        cmd.Parameters.AddWithValue("@Region", mod.Region);
                        cmd.Parameters.AddWithValue("@Area", mod.Area);
                        cmd.Parameters.AddWithValue("@AreaCode", mod.AreaCode);
                        cmd.Parameters.AddWithValue("@DivCode", mod.DivCode);
                        cmd.Parameters.AddWithValue("@ZoneCode", mod.ZoneCode);
                        cmd.Parameters.AddWithValue("@reqStatus", "DISAPPROVED");
                        cmd.Parameters.AddWithValue("@isApproved", mod.isApproved);
                        cmd.Parameters.AddWithValue("@isDivRequest", mod.isDivRequest);
                        cmd.Parameters.AddWithValue("@ClosedDate", syscreated.ToString(format));
                        cmd.Parameters.AddWithValue("@ClosedBy", ss.s_usr_id);
                        cmd.Parameters.AddWithValue("@Remarks", "DISAPPROVED");
                        cmd.Parameters.AddWithValue("@syscreated", syscreated.ToString(format));
                        cmd.Parameters.AddWithValue("@syscreator", ss.s_usr_id);
                        cmd.Parameters.AddWithValue("@forPresident", mod.forPresident);
                        cmd.Parameters.AddWithValue("@deptCode", mod.DeptCode);

                        #endregion Insert to close

                        cmd.CommandText = InsertClose;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        mod.Approver = newDivApprover;

                        switch (mod.Approver)
                        {
                            #region Disapprover selector

                            case "DM":
                                mod.E_DMName = ss.s_fullname;
                                mod.E_DMDate = syscreated.ToString(format);
                                break;

                            case "LocalDiv":
                                mod.E_LocalDivName = ss.s_fullname;
                                mod.E_LocalDivDate = syscreated.ToString(format);
                                break;

                            case "AM":
                                mod.E_AMName = ss.s_fullname;
                                mod.E_AMDate = syscreated.ToString(format);
                                break;

                            case "RM":
                                mod.E_RMName = ss.s_fullname;
                                mod.E_RMDate = syscreated.ToString(format);
                                break;

                            case "VPAssistant":
                                mod.E_VPAssistantName = ss.s_fullname;
                                mod.E_VPAssistantDate = syscreated.ToString(format);
                                break;

                            case "GM":
                                mod.E_GMName = ss.s_fullname;
                                mod.E_GMDate = syscreated.ToString(format);
                                break;

                            case "Pres":
                                mod.E_PresName = ss.s_fullname;
                                mod.E_PresDate = syscreated.ToString(format);
                                break;

                            case "Div1":
                                mod.E_DivName = ss.s_fullname;
                                mod.E_DivDate = syscreated.ToString(format);
                                break;

                            case "Div2":
                                mod.E_Div2Name = ss.s_fullname;
                                mod.E_Div2Date = syscreated.ToString(format);
                                break;

                            case "Div3":
                                mod.E_Div3Name = ss.s_fullname;
                                mod.E_Div3Date = syscreated.ToString(format);
                                break;

                            default:
                                break;

                            #endregion Disapprover selector
                        }

                        mod.reqNumber = RequestNo;
                        disp.UpdateEscalationRemarks(mod, "DISAPPROVED", ss);
                        cmd.Parameters.Clear();
                        cmd.CommandText = "DELETE FROM OnlineRequest.onlineRequest_Open WHERE reqNumber = @reqNo ";
                        cmd.Parameters.AddWithValue("@reqNo", RequestNo);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        log.Info("[Multiple Approving] Request with request no." + RequestNo + " has been disapproved by : " + ss.s_fullname + " . ");
                    }
                }
                return "success";
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return x.ToString();
            }
        }

        private string CheckApproverDiv(string RequestNo, string sessionDivID, string CostCenter)
        {
            try
            {
                string divApprover = string.Empty;
                string isApprovedLocalDiv = "", DivCode1 = "", DivCode2 = "", DivCode3 = "", isDivRequest = "", DeptCode = "";
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    String x = " SELECT a.reqNumber, a.reqCreator, a.TypeID, a.DeptCode, a.isDivRequest, " +
                               " b.DivCode1, b.DivCode2, b.DivCode3,c.isApprovedDM, c.isApprovedLocalDiv, c.isApprovedDiv1, c.isApprovedDiv2, c.isApprovedDiv3 ,forPresident" +
                               " FROM onlineRequest_Open a " +
                               " INNER JOIN requestType b ON a.TypeID = b.TypeID " +
                               " INNER JOIN requestApproverStatus c ON c.reqNumber = a.reqNumber " +
                               " WHERE a.reqNumber=@reqNumber;";
                    var cmd = new MySqlCommand(x, conn);
                    cmd.Parameters.AddWithValue("@reqNumber", RequestNo);
                    conn.Open();

                    using (MySqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (read.HasRows)
                        {
                            read.Read();
                            isApprovedLocalDiv = read["isApprovedLocalDiv"].ToString().Trim();
                            DivCode1 = read["DivCode1"].ToString().Trim();
                            DivCode2 = read["DivCode2"].ToString().Trim();
                            DivCode3 = read["DivCode3"].ToString().Trim();
                            isDivRequest = read["isDivRequest"].ToString().Trim();
                            DeptCode = read["DeptCode"].ToString().Trim();

                            if (isDivRequest == "1")
                            {
                                if (isApprovedLocalDiv == "0" && DeptCode == CostCenter)
                                {
                                    divApprover = "LocalDiv";
                                }
                                else if (DivCode1 == sessionDivID)
                                {
                                    divApprover = "Div1";
                                }
                                else if (DivCode2 == sessionDivID)
                                {
                                    divApprover = "Div2";
                                }
                                else if (DivCode3 == sessionDivID)
                                {
                                    divApprover = "Div3";
                                }
                            }
                            else
                            {
                                if (DivCode1 == sessionDivID)
                                {
                                    divApprover = "Div1";
                                }
                                else if (DivCode2 == sessionDivID)
                                {
                                    divApprover = "Div2";
                                }
                                else if (DivCode3 == sessionDivID)
                                {
                                    divApprover = "Div3";
                                }
                            }
                        }
                    }
                }

                return divApprover;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return "Error";
            }
        }
    }
}