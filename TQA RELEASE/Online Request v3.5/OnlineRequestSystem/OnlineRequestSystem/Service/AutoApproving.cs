using MySql.Data.MySqlClient;
using OnlineRequestSystem.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OnlineRequestSystem.Service
{
    public class AutoApproving
    {
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;
        private string format = "yyyy/MM/dd HH:mm:ss";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AutoApproving));

        internal bool AreaManager(MySqlConnection conn, MySqlCommand cmd, ORtoMySql db, MySqlTransaction tran, CreateReqModels Data, List<RequestTypeMenu> List, ORSession ss)
        {
            bool x = false;
            try
            {
                if (List[0].isAMApproval == 1)
                {
                    conn = db.getConnection();
                    cmd = conn.CreateCommand(); 
                    cmd.CommandText = " INSERT INTO onlineRequest_Escalation ( reqNumber, EscalationAM_Name, EscalationAM_Date, EscalationAM_Remarks ) " +
                                      " VALUES ( @reqno, @param0, @param1, @param2 ) ";
                    cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                    cmd.Parameters.AddWithValue("@param0", ss.s_fullname);
                    cmd.Parameters.AddWithValue("@param1", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@param2", "Approved by Area Manager");
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

              
                    cmd.CommandText = " INSERT INTO requestApproverStatus ( reqNumber, AM_Approver, AM_Approved_Date, isApprovedAM ) " +
                                      " VALUES ( @reqno, @param0, @param1, @param2 )";
                    cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                    cmd.Parameters.AddWithValue("@param0", ss.s_usr_id);
                    cmd.Parameters.AddWithValue("@param1", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@param2", 1);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    x = true;
                }
            }
            catch (Exception error)
            {
                log.Fatal("Error Message : " + error.Message, error);
                x = false;
                throw;
            }
            return x;
        }

        internal bool RegionalManager(MySqlConnection conn, MySqlCommand cmd, ORtoMySql db, MySqlTransaction tran, CreateReqModels Data, List<RequestTypeMenu> List, ORSession ss)
        {
            bool x = false;
            try
            {
                conn = db.getConnection();
                cmd = conn.CreateCommand();
                if (List[0].isAMApproval == 1 && List[0].isRMApproval == 0)
                {
                    cmd.CommandText = " INSERT INTO onlineRequest_Escalation ( reqNumber, EscalationAM_Name, EscalationAM_Date, EscalationAM_Remarks ) " +
                                      " VALUES ( @reqno, @name, @date, @remarks ) ";
                }
                else if (List[0].isAMApproval == 1 && List[0].isRMApproval == 1)
                {
                    cmd.CommandText = " INSERT INTO onlineRequest_Escalation ( " +
                                      "  reqNumber, EscalationAM_Name, EscalationAM_Date, EscalationAM_Remarks, " +
                                      "  EscalationRM_Name, EscalationRM_Date, EscalationRM_Remarks  " +
                                      " ) VALUES ( @reqno, @name, @date, @remarks, @name, @date, @remarks) ";
                }

                cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                cmd.Parameters.AddWithValue("@name", ss.s_fullname);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@remarks", "Approved by Regional Manager.");
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
     
                if (List[0].isAMApproval == 1 && List[0].isRMApproval == 0)
                {
                    cmd.CommandText = " INSERT INTO requestApproverStatus ( reqNumber, AM_Approver, AM_Approved_Date, isApprovedAM ) " +
                                      " VALUES ( @reqno, @usr_id, @date, @isapproved )";
                }
                else if (List[0].isAMApproval == 1 && List[0].isRMApproval == 1)
                {
                    cmd.CommandText = " INSERT INTO requestApproverStatus ( " +
                                      " reqNumber, AM_Approver, AM_Approved_Date, isApprovedAM,  " +
                                      " RM_Approver, RM_Approved_Date, isApprovedRM " +
                                      " ) VALUES ( @reqno, @usr_id, @date, @isapproved, @usr_id, @date, @isapproved)";
                }
                cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                cmd.Parameters.AddWithValue("@usr_id", ss.s_usr_id);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@isapproved", 1);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                x = true;
            }
            catch (Exception error)
            {
                log.Fatal("Error Message : " + error.Message, error);
                x = false;
                throw;
            }
            return x;
        }

        internal bool DepartmentManager(MySqlConnection conn, MySqlCommand cmd, ORtoMySql db, MySqlTransaction tran, CreateReqModels Data, List<RequestTypeMenu> List, ORSession ss)
        {
            bool x = false;
            try
            {
                conn = db.getConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = " INSERT INTO onlineRequest_Escalation ( reqNumber, EscalationDM_Name, EscalationDM_Date, EscalationDM_remarks " +
                                  " )  VALUES (@reqno, @name, @date, @remarks)";
                cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                cmd.Parameters.AddWithValue("@name", ss.s_fullname);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@remarks", "Approved by Department Manager.");
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                cmd.CommandText = " INSERT INTO requestApproverStatus ( " +
                                  " reqNumber, DM_Approver, DM_Approved_Date, isApprovedDM ) " +
                                  " VALUES (@reqno, @usr_id, @date, @isapproved)";
                cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                cmd.Parameters.AddWithValue("@usr_id", ss.s_usr_id);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@isapproved", 1);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                x = true;
            }
            catch (Exception error)
            {
                log.Fatal("Error Message : " + error.Message, error);
                x = false;
                throw;
            }
            return x;
        }

        internal bool DivisionManager(MySqlConnection conn, MySqlCommand cmd, ORtoMySql db, MySqlTransaction tran, CreateReqModels Data, List<RequestTypeMenu> List, ORSession ss)
        {
            bool x = false;

            try
            {
                if (List[0].Approval_DivCode1 == ss.s_DivisionID)
                {
                    cmd.CommandText = " INSERT INTO onlineRequest_Escalation ( reqNumber, " +
                                      " EscalationDM_Name, EscalationDM_Date, EscalationDM_remarks, " +
                                      " EscalationLocalDiv_Name, EscalationLocalDiv_Date, EscalationLocalDiv_Remarks, " +
                                      " EscalationDiv_Name, EscalationDiv_Date, EscalationDiv_Remarks " +
                                      " )  VALUES ( @reqno , @name, @date, @remarks, @name, @date, @remarks, @name, @date, @remarks )";
                }
                else
                {
                    cmd.CommandText = " INSERT INTO onlineRequest_Escalation ( reqNumber, " +
                                      " EscalationDM_Name, EscalationDM_Date, EscalationDM_remarks, " +
                                      " EscalationLocalDiv_Name, EscalationLocalDiv_Date, EscalationLocalDiv_Remarks " +
                                      " )  VALUES ( @reqno, @name, @date, @remarks , @name, @date, @remarks )";
                }

                cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                cmd.Parameters.AddWithValue("@name", ss.s_fullname);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@remarks", "Approved by Division Manager.");
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();


                if (List[0].Approval_DivCode1 == ss.s_DivisionID)
                {
                    cmd.CommandText = " INSERT INTO requestApproverStatus ( reqNumber, " +
                                      " DM_Approver, DM_Approved_Date, isApprovedDM, " +
                                      " LocalDiv_Approver, LocalDiv_Approved_Date, isApprovedLocalDiv, " +
                                      " Div_Approver1, DivCode1, Div_Approved_Date1, isApprovedDiv1 " +
                                      " )  VALUES ( @reqno, @usr_id, @date, @isapproved, @usr_id, @date, @isapproved, @usr_id , @divcode, @date, @isapproved ) ";

                    cmd.Parameters.AddWithValue("@divcode", ss.s_DivCode);
                }
                else
                {
                    cmd.CommandText = " INSERT INTO requestApproverStatus ( reqNumber, " +
                                      " DM_Approver, DM_Approved_Date, isApprovedDM, " +
                                      " LocalDiv_Approver, LocalDiv_Approved_Date, isApprovedLocalDiv " +
                                      " )  VALUES ( @reqno, @usr_id, @date, @isapproved, @usr_id, @date, @isapproved ) ";
                }
                cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                cmd.Parameters.AddWithValue("@usr_id", ss.s_usr_id);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@isapproved", 1);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                x = true;
            }
            catch (Exception error)
            {
                log.Fatal("Error Message : " + error.Message, error);
                x = false;
                throw;
            }
            return x;
        }

        internal bool GMOGenman(MySqlConnection conn, MySqlCommand cmd, ORtoMySql db, MySqlTransaction tran, CreateReqModels Data, List<RequestTypeMenu> List, ORSession ss)
        {
            bool x = false;

            try
            {

                if (List[0].Approval_DivCode1 == ss.s_DivisionID)
                {
                    cmd.CommandText = " INSERT INTO onlineRequest_Escalation ( reqNumber, " +
                                      " EscalationDM_Name, EscalationDM_Date, EscalationDM_remarks, " +
                                      " EscalationLocalDiv_Name, EscalationLocalDiv_Date, EscalationLocalDiv_Remarks, " +
                                      " EscalationDiv_Name, EscalationDiv_Date, EscalationDiv_Remarks " +
                                      " )  VALUES ( @reqno , @name, @date, @remarks, @name, @date, @remarks, @name, @date, @remarks )";
                }
                else
                {
                    cmd.CommandText = " INSERT INTO onlineRequest_Escalation ( reqNumber, " +
                                      " EscalationDM_Name, EscalationDM_Date, EscalationDM_remarks, " +
                                      " EscalationLocalDiv_Name, EscalationLocalDiv_Date, EscalationLocalDiv_Remarks " +
                                      " )  VALUES ( @reqno, @name, @date, @remarks , @name, @date, @remarks )";
                }

                cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                cmd.Parameters.AddWithValue("@name", ss.s_fullname);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@remarks", "Approved by Vice President.");
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();


                if (List[0].Approval_DivCode1 == ss.s_DivisionID)
                {
                    cmd.CommandText = " INSERT INTO requestApproverStatus ( reqNumber, " +
                                      " DM_Approver, DM_Approved_Date, isApprovedDM, " +
                                      " LocalDiv_Approver, LocalDiv_Approved_Date, isApprovedLocalDiv, " +
                                      " Div_Approver1, DivCode1, Div_Approved_Date1, isApprovedDiv1 " +
                                      " )  VALUES ( @reqno, @usr_id, @date, @isapproved, @usr_id, @date, @isapproved, @usr_id , @divcode, @date, @isapproved ) ";
                    cmd.Parameters.AddWithValue("@divcode", ss.s_DivCode);
                }
                else
                {
                    cmd.CommandText = " INSERT INTO requestApproverStatus ( reqNumber, " +
                                      " DM_Approver, DM_Approved_Date, isApprovedDM, " +
                                      " LocalDiv_Approver, LocalDiv_Approved_Date, isApprovedLocalDiv " +
                                      " )  VALUES ( @reqno, @usr_id, @date, @isapproved, @usr_id, @date, @isapproved ) ";
                }
                cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                cmd.Parameters.AddWithValue("@usr_id", ss.s_usr_id);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@isapproved", 1);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                x = true;
            }
            catch (Exception error)
            {
                log.Fatal("Error Message : " + error.Message, error);
                x = false;
                throw;
            }
            return x;
        }

        internal bool President(MySqlConnection conn, MySqlCommand cmd, ORtoMySql db, MySqlTransaction tran, CreateReqModels Data, List<RequestTypeMenu> List, ORSession ss)
        {
            bool x = false;

            try
            {

                if (List[0].Approval_DivCode1 == ss.s_DivisionID)
                {
                    cmd.CommandText = " INSERT INTO onlineRequest_Escalation ( reqNumber, " +
                                      " EscalationDM_Name, EscalationDM_Date, EscalationDM_remarks, " +
                                      " EscalationLocalDiv_Name, EscalationLocalDiv_Date, EscalationLocalDiv_Remarks, " +
                                      " EscalationDiv_Name, EscalationDiv_Date, EscalationDiv_Remarks " +
                                      " )  VALUES ( @reqno , @name, @date, @remarks, @name, @date, @remarks, @name, @date, @remarks )";
                }
                else
                {
                    cmd.CommandText = " INSERT INTO onlineRequest_Escalation ( reqNumber, " +
                                      " EscalationDM_Name, EscalationDM_Date, EscalationDM_remarks, " +
                                      " EscalationLocalDiv_Name, EscalationLocalDiv_Date, EscalationLocalDiv_Remarks " +
                                      " )  VALUES ( @reqno, @name, @date, @remarks , @name, @date, @remarks )";
                }

                cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                cmd.Parameters.AddWithValue("@name", ss.s_fullname);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@remarks", "Approved by President.");
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();


                if (List[0].Approval_DivCode1 == ss.s_DivisionID)
                {
                    cmd.CommandText = " INSERT INTO requestApproverStatus ( reqNumber, " +
                                      " DM_Approver, DM_Approved_Date, isApprovedDM, " +
                                      " LocalDiv_Approver, LocalDiv_Approved_Date, isApprovedLocalDiv, " +
                                      " Div_Approver1, DivCode1, Div_Approved_Date1, isApprovedDiv1 " +
                                      " )  VALUES ( @reqno, @usr_id, @date, @isapproved, @usr_id, @date, @isapproved, @usr_id , @divcode, @date, @isapproved ) ";
                    cmd.Parameters.AddWithValue("@divcode", ss.s_DivCode);
                }
                else
                {
                    cmd.CommandText = " INSERT INTO requestApproverStatus ( reqNumber, " +
                                      " DM_Approver, DM_Approved_Date, isApprovedDM, " +
                                      " LocalDiv_Approver, LocalDiv_Approved_Date, isApprovedLocalDiv" +
                                      " )  VALUES ( @reqno, @usr_id, @date, @isapproved, @usr_id, @date, @isapproved ) ";
                }
                cmd.Parameters.AddWithValue("@reqno", Data.RequestNo);
                cmd.Parameters.AddWithValue("@usr_id", ss.s_usr_id);
                cmd.Parameters.AddWithValue("@date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@isapproved", 1);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                x = true;
            }
            catch (Exception error)
            {
                log.Fatal("Error Message : " + error.Message, error);
                x = false;
                throw;
            }

            return x;
        }
    }
}