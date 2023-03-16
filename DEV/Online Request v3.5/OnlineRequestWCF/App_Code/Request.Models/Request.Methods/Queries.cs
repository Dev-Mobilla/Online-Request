using System;
using System.Collections.Generic;
using System.Linq;
using OnlineRequestWCF.Request.Models;
using System.Web;
using System.Data.SqlClient;
using log4net;
using System.Globalization;
using System.Configuration;
using System.Data;
using log4net.Config;

namespace OnlineRequestWCF.Request.Methods
{
    internal class Queries
    {
        Helper h = new Helper();
        private readonly ILog _log;
        public Queries(ILog RequestLog)
        {
            XmlConfigurator.Configure();
            _log = RequestLog;
        }
        protected internal LoginResponse Authentication(string username, string password)
        {
            TextInfo toTC = new CultureInfo("en-US", false).TextInfo;
            var model = new UserInformation();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("exec IR_SP_LogIn_v3 @USERNAME, @PASSWORD", con);
                    cmd.Parameters.AddWithValue("@USERNAME", username);
                    cmd.Parameters.AddWithValue("@PASSWORD", password);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            model.s_fullname = toTC.ToTitleCase(rdr["fullname"].ToString().Trim().ToLower());
                            model.s_usr_id = username.Trim().ToUpper();
                            model.s_res_id = rdr["res_id"].ToString().Trim();
                            model.s_comp = rdr["comp"].ToString().Trim();
                            model.s_costcenter = rdr["costcenter"].ToString().Trim();
                            if ((new[] { "0001MT", "001MIS", "01MISNCC", "01MISHD", "01MISCRM", "01MISDBA", 
                                "01MISCC", "01MISLO", "01MISOA", "01MISUS", "01MISENT", "01MISTSG", "0001BOS" }).Contains(model.s_costcenter))
                            {
                                model.s_costcenter = "0001MIS";
                            }
                            model.s_zonecode = rdr["zonecode"].ToString().Trim();
                            if (model.s_zonecode == "SHOWROOM")
                            {
                                model.s_zonecode = rdr["class_02"].ToString().Trim();
                            }
                            model.s_Division = rdr["division"].ToString().Trim();
                            model.s_DivCode = rdr["divisioncode"].ToString().Trim();
                            model.s_DivAcro = rdr["divisionacro"].ToString().Trim();
                            model.s_DivManager = rdr["divisionmanager"].ToString().Trim();
                            model.s_job_title = rdr["job_title"].ToString().Trim();
                            model.s_task = rdr["task"].ToString().Trim();
                            model.s_blocked = rdr["blocked"].ToString().Trim();
                            model.s_bedrnm = rdr["bedrnm"].ToString().Trim();
                            model.s_region = rdr["region"].ToString().Trim();
                            model.s_regionCode = rdr["regioncode"].ToString().Trim();
                            model.s_areaCode = rdr["AreaCode"].ToString().Trim();
                            model.s_BMName = rdr["BMName"].ToString().Trim();
                            model.s_AMName = rdr["AMName"].ToString().Trim();
                            model.s_LPTLName = rdr["LPTLName"].ToString().Trim();
                            model.s_RMName = rdr["RMName"].ToString().Trim();
                            model.s_area = getArea(model.s_areaCode, model.s_region, model.s_zonecode);
                            if (model.s_blocked == "1")
                            {
                                return new LoginResponse { resCode = "1", resMsg = resMessages(11) };
                            }
                        }
                        else
                        {
                            return new LoginResponse { resCode = "1", resMsg = resMessages(7) };
                        }
                    }
                }
                return new LoginResponse { resCode = "0", resMsg = resMessages(1), logdata = model };
            }
            catch (SqlException e)
            {
                _log.Fatal(e.Message, e);
                return new LoginResponse { resCode = "1", resMsg = resMessages(0) };
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new LoginResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception e)
            {
                _log.Fatal(e.Message, e);
                return new LoginResponse { resCode = "1", resMsg = resMessages(4) };
            }
        }
        public string getArea(string AreaCode, string Region, string ZoneCode)
        {
            try
            {
                string Area = "";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "select Area from IRAreaCode Where AreaCode = @Area and region = @Region and zonecode = @zoneCode ";
                    cmd.Parameters.AddWithValue("@Area", AreaCode);
                    cmd.Parameters.AddWithValue("@Region", Region);
                    cmd.Parameters.AddWithValue("@zoneCode", ZoneCode);
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.Read())
                        {
                            Area = rdr["Area"].ToString().Trim();
                        }
                    }
                    return Area;
                }
            }
            catch (Exception x)
            {
                _log.Error(x.Message, x);
                throw;
            }
        }
        protected internal BranchNameResponse GetBranchName(string BranchCode, string Region, string ZoneCode)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SELECT bedrnm FROM WebBranches WHERE bedrnr = @BranchCode and zonecode = @ZoneCode and region = @Region", con);
                    cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                    cmd.Parameters.AddWithValue("@Region", Region);
                    cmd.Parameters.AddWithValue("@ZoneCode", ZoneCode);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            return new BranchNameResponse { BranchName = rdr["bedrnm"].ToString().Trim(), resCode = "0", resMsg = resMessages(1) };
                        }
                        else
                        {
                            return new BranchNameResponse { BranchName = string.Empty, resCode = "0", resMsg = resMessages(1) };
                        }
                    }
                }
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new BranchNameResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new BranchNameResponse { resCode = "1", resMsg = resMessages(0) };
            }
        }
        protected internal DivisionNameResponse GetDivisionName(string divcode)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select Division from IRDivision where divisioncode = @DivCode ";
                        cmd.Parameters.AddWithValue("@DivCode", divcode);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                return new DivisionNameResponse { DivisionName = rdr["Division"].ToString().Trim(), resCode = "0", resMsg = resMessages(1) };
                            }
                            else
                            {
                                return new DivisionNameResponse { DivisionName = string.Empty, resCode = "0", resMsg = resMessages(1) };
                            }
                        }
                    }
                }
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new DivisionNameResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new DivisionNameResponse { resCode = "1", resMsg = resMessages(0) };
            }
        }
        protected internal BranchCodeResponses GetBranchCode(string branchName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select top 1 bedrnr FROM WebProjects.dbo.WebBranches WHERE bedrnm = @branchName";
                        cmd.Parameters.AddWithValue("@branchName", branchName);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                return new BranchCodeResponses { BranchCode = rdr["bedrnr"].ToString().Trim(), resCode = "0", resMsg = resMessages(1) };
                            }
                            else
                            {
                                return new BranchCodeResponses { BranchCode = string.Empty, resCode = "0", resMsg = resMessages(1) };
                            }
                        }
                    }
                }
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new BranchCodeResponses { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new BranchCodeResponses { resCode = "1", resMsg = resMessages(0) };
            }
        }
        protected internal DivisionCostCenterResponses GetDivisionCostCenter(string divacro)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        string GetDivCostCenter = "select CostCenter from IRDivision Where DivisionAcro = @divacro";
                        cmd.Parameters.AddWithValue("@divacro", divacro);
                        cmd.CommandText = GetDivCostCenter;
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                return new DivisionCostCenterResponses { DivisionCostCenter = rdr["CostCenter"].ToString().Trim(), resCode = "0", resMsg = resMessages(1) };
                            }
                            else
                            {
                                return new DivisionCostCenterResponses { DivisionCostCenter = string.Empty, resCode = "0", resMsg = resMessages(1) };
                            }
                        }
                    }
                }
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new DivisionCostCenterResponses { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new DivisionCostCenterResponses { resCode = "1", resMsg = resMessages(0) };
            }
        }
        protected internal IRDivCodeResponses GetIRDivCode(string DivCode)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select DivisionCode from IRDivision Where DivisionAcro = @DivCode";
                        cmd.Parameters.AddWithValue("@DivCode", DivCode);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                return new IRDivCodeResponses { IRDivCode = rdr["DivisionCode"].ToString().Trim(), resCode = "0", resMsg = resMessages(1) };
                            }
                            else
                            {
                                return new IRDivCodeResponses { IRDivCode = string.Empty, resCode = "0", resMsg = resMessages(1) };
                            }
                        }
                    }
                }
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new IRDivCodeResponses { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new IRDivCodeResponses { resCode = "1", resMsg = resMessages(0) };
            }
        }
        protected internal DivManResourceIDResponses GetDivisionApproverResourceID(string zonecode, string DivisionManager)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        string ResID = "select * from webAccounts where fullname = @divMngr and zonecode = @zcode ";
                        cmd.Parameters.AddWithValue("@divMngr", DivisionManager);
                        cmd.Parameters.AddWithValue("@zcode", zonecode);
                        cmd.CommandText = ResID;
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                return new DivManResourceIDResponses { DivisionApproverResouce = rdr["usr_id"].ToString().Trim(), resCode = "0", resMsg = resMessages(1) };
                            }
                            else
                            {
                                return new DivManResourceIDResponses { DivisionApproverResouce = string.Empty, resCode = "0", resMsg = resMessages(1) };
                            }
                        }
                    }
                }
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new DivManResourceIDResponses { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new DivManResourceIDResponses { resCode = "1", resMsg = resMessages(0) };
            }
        }

        protected internal DivisionDetailsResponses GetDivisionDetails(string DivAcro)
        {
            try
            {
                var data = new DivisionDetails();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    SqlCommand cmd = con.CreateCommand();
                    con.Open();
                    cmd.CommandText = "select * from IRDivision WHERE DivisionAcro = @divAcro";
                    cmd.Parameters.AddWithValue("@divAcro", DivAcro);

                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    rdr.Read();
                    data.DivisionEscalation = rdr["DivisionEscalation"].ToString().Trim();
                    data.Division = rdr["Division"].ToString().Trim();
                    data.DivisionCode = rdr["DivisionCode"].ToString().Trim();
                    data.DivisionAcro = rdr["DivisionAcro"].ToString().Trim();
                    data.ZoneCode = rdr["ZoneCode"].ToString().Trim();
                    data.CostCenter = rdr["CostCenter"].ToString().Trim();
                    data.DivisionManager = rdr["DivisionManager"].ToString().Trim();
                    data.ContactNum = rdr["ContactNum"].ToString().Trim();
                }
                return new DivisionDetailsResponses { DivisionDetailsResp = data };
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new DivisionDetailsResponses { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception e)
            {

                _log.Fatal(e.Message, e);
                return new DivisionDetailsResponses { resCode = "1", resMsg = resMessages(4) };
            }
        }

        protected internal DivisionAcroResponses GetDivisionAcro(string divName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select DivisionAcro from IRDivision WHERE DivisionEscalation = @divEscalation ";
                        cmd.Parameters.AddWithValue("@divEscalation", divName);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                return new DivisionAcroResponses { DivisionAcro = rdr["DivisionAcro"].ToString().Trim(), resCode = "0", resMsg = resMessages(1) };
                            }
                            else
                            {
                                return new DivisionAcroResponses { DivisionAcro = string.Empty, resCode = "0", resMsg = resMessages(1) };
                            }
                        }
                    }
                }
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new DivisionAcroResponses { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new DivisionAcroResponses { resCode = "1", resMsg = resMessages(0) };
            }
        }

        protected internal ListOfBranchesResponse GetListOfBranches(string zonecode)
        {
            try
            {
                var List = new ListOfBranchesResponse();
                List.ListOfBranches = new List<ListOfBranches>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        string qZone = h.ZoneCodeStringSelector(zonecode);
                        cmd.CommandText = "SELECT DISTINCT(bedrnm) FROM WebBranches WHERE " + qZone + " ORDER BY bedrnm";
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                List.ListOfBranches.Add(new ListOfBranches { BranchName = rdr["bedrnm"].ToString().Trim(), BranchNameValue = rdr["bedrnm"].ToString().Trim() });
                            }
                        }
                    }
                }
                var res = new ListOfBranchesResponse { ListOfBranches = List.ListOfBranches, resCode = "0", resMsg = resMessages(1) };
                return res;
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new ListOfBranchesResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new ListOfBranchesResponse { resCode = "1", resMsg = resMessages(0) };
            }
        }

        protected internal ListOfDivisionResponse GetListOfDivisions()
        {
            try
            {
                var List = new ListOfDivisionResponse();
                List.ListOfDivisions = new List<ListOfDivisions>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT DivisionEscalation, DivisionCode  FROM IRDivision ORDER BY Division";
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                List.ListOfDivisions.Add(new ListOfDivisions { DivisionEscalation = rdr["DivisionEscalation"].ToString().Trim(), DivisionCode = rdr["DivisionCode"].ToString().Trim() });
                            }
                        }
                    }
                }
                var res = new ListOfDivisionResponse { ListOfDivisions = List.ListOfDivisions, resCode = "0", resMsg = resMessages(1) };
                return res;
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new ListOfDivisionResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new ListOfDivisionResponse { resCode = "1", resMsg = resMessages(0) };
            }
        }


        protected internal ListOfAreaResponse GetListOfAreas(string zonecode)
        {
            try
            {
                var List = new ListOfAreaResponse();
                List.ListOfAreas = new List<ListOfAreas>();
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        string azone = h.AreaStringSelect(zonecode);
                        cmd.CommandText = "SELECT Area FROM IRAreaCode WHERE " + azone + " ORDER BY Area";
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                List.ListOfAreas.Add(new ListOfAreas { Area = rdr["Area"].ToString().Trim(), AreaValue = rdr["Area"].ToString().Trim() });
                            }
                        }
                    }
                }
                var res = new ListOfAreaResponse { ListOfAreas = List.ListOfAreas, resCode = "0", resMsg = resMessages(1) };
                return res;
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new ListOfAreaResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new ListOfAreaResponse { resCode = "1", resMsg = resMessages(0) };
            }
        }
        protected internal ListOfRegionResponse GetListOfRegions(string zonecode)
        {
            try
            {
                var List = new ListOfRegionResponse();
                List.ListOfRegions = new List<ListOfRegions>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        string azone = h.AreaStringSelect(zonecode);
                        cmd.CommandText = "SELECT Region FROM IRRegionCode WHERE " + azone + " ORDER BY region";
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                List.ListOfRegions.Add(new ListOfRegions { Region = rdr["Region"].ToString().Trim(), RegionValue = rdr["Region"].ToString().Trim() });
                            }
                        }
                    }
                }
                var res = new ListOfRegionResponse { ListOfRegions = List.ListOfRegions, resCode = "0", resMsg = resMessages(1) };
                return res;
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new ListOfRegionResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new ListOfRegionResponse { resCode = "1", resMsg = resMessages(0) };
            }
        }

        protected internal ListOfSDCResponse GetListOfSDC(string CostCenter)
        {
            try
            {
                var List = new ListOfSDCResponse();
                List.ListOfSDC = new List<SDCDetails>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        string Query = "";
                        if (CostCenter == "0001MMD")
                        {
                            Query = "SELECT zonecode as ZoneCode, class_03 as Region,fullname as RM,comp as BranchCode,bedrnm as BranchName " +
                                      "FROM IRRegionalManagers WHERE zonecode in ('VISAYAS','MINDANAO')";
                        }
                        else
                        {
                            Query = "SELECT zonecode as ZoneCode, class_03 as Region,fullname as RM,comp as BranchCode,bedrnm as BranchName " +
                                       "FROM IRRegionalManagers WHERE zonecode in ('LUZON','NCR')";
                        }
                        cmd.CommandText = Query;
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                List.ListOfSDC.Add(new SDCDetails
                                {
                                    ZoneCode = rdr["ZoneCode"].ToString().Trim(),
                                    Region = rdr["Region"].ToString().Trim(),
                                    RM = rdr["RM"].ToString().Trim(),
                                    BranchCode = rdr["BranchCode"].ToString().Trim(),
                                    BranchName = rdr["BranchName"].ToString().Trim()
                                });
                            }
                        }
                    }
                }
                var res = new ListOfSDCResponse { ListOfSDC = List.ListOfSDC, resCode = "0", resMsg = resMessages(1) };
                return res;
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new ListOfSDCResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new ListOfSDCResponse { resCode = "1", resMsg = resMessages(0) };
            }
        }

        private string resMessages(int code)
        {
            string retMsg = "";
            switch (code)
            {
                case 0:
                    return retMsg = "Service Error";
                case 1:
                    return retMsg = "Success!";
                case 2:
                    return retMsg = "No Data Found!";
                case 3:
                    return retMsg = "You are unauthorized to use this application.";
                case 4:
                    return retMsg = "Please review your data and try again.";
                case 5:
                    return retMsg = "Unable to process request. Connection timeout occured. Please try again later.";
                case 6:
                    return retMsg = "Unable to process request. Failed in connecting to server. Please try again later.";
                case 7:
                    return retMsg = "The username or password is incorrect. Please try again.";
                case 8:
                    return retMsg = "Unable to process request. Some record's already exist.";
                case 9:
                    return retMsg = "Unable to send request message";
                case 10:
                    return retMsg = "Request Message Sent!";
                case 11:
                    return retMsg = "Your account is blocked.";
                default:
                    return retMsg;
            }
        }

        protected internal ListOfIRDivisionResponse ListIRDivisions(string zonecode)
        {
            try
            {
                var List = new ListOfIRDivisionResponse();
                List.ListIRDivisions = new List<ListOfIRDivisions>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        string zcode = h.DivisionZonecode(zonecode);
                        cmd.CommandText = "SELECT DivisionEscalation, CostCenter FROM IRDivision WHERE ZoneCode = '" + zcode + "' ORDER BY Division";
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                List.ListIRDivisions.Add(new ListOfIRDivisions { DivisionName = rdr["DivisionEscalation"].ToString().Trim(), CostCenter = rdr["CostCenter"].ToString().Trim() });
                            }
                        }
                    }
                }
                var res = new ListOfIRDivisionResponse { ListIRDivisions = List.ListIRDivisions, resCode = "0", resMsg = resMessages(1) };
                return res;
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new ListOfIRDivisionResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new ListOfIRDivisionResponse { resCode = "1", resMsg = resMessages(0) };
            }
        }

        protected internal DivisionNameResponse GetIRDivisionName(string costcenter)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select Division from IRDivision where CostCenter = @DivCode ";
                        cmd.Parameters.AddWithValue("@DivCode", costcenter);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                return new DivisionNameResponse { DivisionName = rdr["Division"].ToString().Trim(), resCode = "0", resMsg = resMessages(1) };
                            }
                            else
                            {
                                return new DivisionNameResponse { DivisionName = string.Empty, resCode = "0", resMsg = resMessages(1) };
                            }
                        }
                    }
                }
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new DivisionNameResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new DivisionNameResponse { resCode = "1", resMsg = resMessages(0) };
            }
        }

        

        //For Item Pricing
        protected internal ListOfItemsResponse GetAllItems()
        {
            try
            {
                var List = new ListOfItemsResponse();
                List.ListOfItems = new List<ListOfItems>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MMDHO"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select ItemCode , Description, SalesPackagePrice from Items";
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                List.ListOfItems.Add(new ListOfItems { ItemCode = rdr["ItemCode"].ToString().Trim(), ItemDescription = rdr["Description"].ToString().Trim(), ItemPrice = rdr["SalesPackagePrice"].ToString().Trim() });
                            }
                        }
                    }
                }
                var res = new ListOfItemsResponse { ListOfItems = List.ListOfItems, resCode = "0", resMsg = resMessages(1) };
                return res;
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new ListOfItemsResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new ListOfItemsResponse { resCode = "1", resMsg = resMessages(0) };
            }

        }

        protected internal ListOfItemsResponse SearchItem(string searchCriteria)
        {
            try
            {
                var List = new ListOfItemsResponse();
                List.ListOfItems = new List<ListOfItems>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MMDHO"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select ItemCode, Description, SalesPackagePrice from Items where Description LIKE '%' + @searchCriteria + '%' OR ItemCode=@searchCriteria";
                        cmd.Parameters.AddWithValue("@searchCriteria", searchCriteria);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                List.ListOfItems.Add(new ListOfItems { ItemCode = rdr["ItemCode"].ToString().Trim(), ItemDescription = rdr["Description"].ToString().Trim(), ItemPrice = rdr["SalesPackagePrice"].ToString().Trim() });
                            }
                        }
                    }
                }
                var res = new ListOfItemsResponse { ListOfItems = List.ListOfItems, resCode = "0", resMsg = resMessages(1) };
                return res;
            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new ListOfItemsResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new ListOfItemsResponse { resCode = "1", resMsg = resMessages(0) };
            }

        }



        #region CASH REQUEST

        protected internal CashRequestApproversResponse CashRequestApprovers(string zonecode, string class_04, string region, string areacode, string job_title)
        {
            try
            {
                var ApproverList = new CashRequestApproversResponse();
                ApproverList.ListOfCashRequestApprovers = new List<ListOfCashRequestApprovers>();


                var approvers = new List<string>();

                _log.Info(zonecode + class_04 + region + areacode);

                var am = GetAreaManager(zonecode, class_04, region, areacode);
                var rm = GetRManager(zonecode, class_04, region, areacode);
                var ram = GetRAManager(zonecode, class_04, region, areacode);
                var asst = GetGmoAstGenman(zonecode);
                var vpo = GetGmoGenman(zonecode);


                if (job_title == "BM" || job_title == "ABM" || job_title == "TELLER" ||
                    job_title == "BM/BOSMAN" || job_title == "LPTL/BM/LPT/BOSMAN" || job_title == "LPT/BM-A/BOSMAN" ||
                    job_title == "ASST. BM" || job_title == "LPT/BM-R" || job_title == "LPT-A/BOSMAN" || job_title == "BRANCH STAFF" ||
                    job_title == "ABM/BOSMAN" || job_title == "LPT/BM-R/BOSMAN" || job_title == "LPT-A" || job_title == "ABM/LPT-A/BOSMAN" ||
                    job_title == "ABM/LPT-A")
                {

                    if (am.Count == 0 || rm.Count == 0 || ram.Count == 0)
                    {
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                    }
                    else
                    {
                        approvers.Add(am[0]);
                        approvers.Add(am[1]);
                        approvers.Add(rm[0]);
                        approvers.Add(rm[1]);
                        approvers.Add(ram[0]);
                        approvers.Add(ram[1]);
                        approvers.Add(asst[0]);
                        approvers.Add(asst[1]);
                        approvers.Add(vpo[0]);
                        approvers.Add(vpo[1]);

                    }

                }
                else if (job_title == "AREA MANAGER")
                {

                    if (rm.Count == 0 || ram.Count == 0)
                    {
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                    }
                    else
                    {
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(rm[0]);
                        approvers.Add(rm[1]);
                        approvers.Add(ram[0]);
                        approvers.Add(ram[1]);
                        approvers.Add(asst[0]);
                        approvers.Add(asst[1]);
                        approvers.Add(vpo[0]);
                        approvers.Add(vpo[1]);
                    }


                }
                else if (job_title == "REGIONAL MAN")
                {

                    if (ram.Count == 0)
                    {
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                    }
                    else
                    {
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(ram[0]);
                        approvers.Add(ram[1]);
                        approvers.Add(asst[0]);
                        approvers.Add(asst[1]);
                        approvers.Add(vpo[0]);
                        approvers.Add(vpo[1]);
                    }


                }
                else if (job_title == "RAM")
                {
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(asst[0]);
                    approvers.Add(asst[1]);
                    approvers.Add(vpo[0]);
                    approvers.Add(vpo[1]);

                }
                else if (job_title == "GMO-ASTGENMAN" || job_title == "ADM ASS SR" || job_title == "GM'S STAFF")
                {
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(vpo[0]);
                    approvers.Add(vpo[1]);
                }
                else if (job_title == "GMO-GENMAN")
                {
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                }

                ApproverList.ListOfCashRequestApprovers.Add(new ListOfCashRequestApprovers
                {
                    //AmName = { new AmCred { amFullname = approvers[0].ToString(), amId = approvers[0].ToString() } },
                    AmName = approvers[0],
                    AmId = approvers[1],
                    RmName = approvers[2],
                    RmId = approvers[3],
                    RamName = approvers[4],
                    RamId = approvers[5],
                    GmoGenAsstName = approvers[6],
                    GmoGenAsstId = approvers[7],
                    GmoGenName = approvers[8],
                    GmoGenId = approvers[9]
                });

                var response = new CashRequestApproversResponse { ListOfCashRequestApprovers = ApproverList.ListOfCashRequestApprovers, resCode = "0", resMsg = resMessages(1) };
                return response;


            }
            catch (TimeoutException e)
            {
                _log.Fatal(e.Message, e);
                return new CashRequestApproversResponse { resCode = "1", resMsg = resMessages(5) };
            }
            catch (Exception x)
            {
                _log.Fatal(x.Message, x);
                return new CashRequestApproversResponse { resCode = "1", resMsg = resMessages(0) };
            }
        }


        #endregion

        #region AREA MANAGER
        private List<string> GetAreaManager(string zonecode, string class_04, string region, string areacode)
        {
            //var amFullname = "";
            //var amId = "";

            var amCred = new List<string>();


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //GET AREA MANAGER
                    //cmd.CommandText = "SELECT wa.fullname AS fullname FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.zonecode WHERE wa.zonecode=@zonecode AND wb.class_04 =@class_04 AND wb.region=@region AND wb.areacode=@areacode AND wa.job_title = 'AREA MANAGER'";
                    cmd.CommandText = "SELECT TOP 1 wa.fullname AS fullname, wa.res_id as resId FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.zonecode WHERE wa.zonecode=@zonecode AND wb.class_03 =@class_03 AND class_04 = @class_04 AND wb.region=@region AND wb.areacode=@areacode AND wa.job_title = 'AREA MANAGER'";

                    cmd.Parameters.AddWithValue("@zonecode", zonecode);
                    cmd.Parameters.AddWithValue("@class_04", class_04);
                    cmd.Parameters.AddWithValue("@class_03", region);
                    cmd.Parameters.AddWithValue("@region", region);
                    cmd.Parameters.AddWithValue("@areacode", areacode);

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();

                            //amFullname = rdr["fullname"].ToString().Trim();
                            //amId = rdr["resId"].ToString().Trim();

                            amCred.Add(rdr["fullname"].ToString().Trim());
                            amCred.Add(rdr["resId"].ToString().Trim());


                        }
                        else
                        {
                            //approvers.Add(string.Empty);
                            //amFullname = string.Empty;
                            //amId = string.Empty;
                            amCred.Add(string.Empty);
                            amCred.Add(string.Empty);

                        }
                        con.Close();
                        rdr.Close();
                    }
                }
            }
            return amCred;
        }
        #endregion

        #region REGIONAL MANAGER
        private List<string> GetRManager(string zonecode, string class_04, string region, string areacode)
        {
            //var rmFullname = "";

            var rmCred = new List<string>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //GET REGIONAL MANAGER
                    //cmd.CommandText = "SELECT wa.fullname AS fullname FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.zonecode WHERE wa.zonecode=@rm_zonecode AND wb.class_04 =@rm_class_04 AND wb.region=@rm_region AND wb.areacode=@rm_areacode AND wa.job_title = 'REGIONAL MAN'";
                    cmd.CommandText = "SELECT TOP 1 wa.fullname AS fullname, wa.res_id as resId FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.zonecode WHERE wa.zonecode=@rm_zonecode AND wb.class_03 =@rm_class_03 AND class_04 = @rm_class_04 AND wb.region=@rm_region AND wb.areacode=@rm_areacode AND wa.job_title = 'REGIONAL MAN'";

                    cmd.Parameters.AddWithValue("@rm_zonecode", zonecode);
                    cmd.Parameters.AddWithValue("@rm_class_04", class_04);
                    cmd.Parameters.AddWithValue("@rm_class_03", region);
                    cmd.Parameters.AddWithValue("@rm_region", region);
                    cmd.Parameters.AddWithValue("@rm_areacode", areacode);

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            rmCred.Add(rdr["fullname"].ToString().Trim());
                            rmCred.Add(rdr["resId"].ToString().Trim());


                        }
                        else
                        {
                            //approvers.Add(string.Empty);
                            rmCred.Add(string.Empty);
                            rmCred.Add(string.Empty);

                        }
                        con.Close();
                        rdr.Close();
                    }
                }
            }
            return rmCred;
        }
        #endregion

        #region REGIONAL AUDIT MANAGER
        private List<string> GetRAManager(string zonecode, string class_04, string region, string areacode)
        {
            //var ramFullname = "";
            var ramCred = new List<string>();


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //GET REGIONAL AUDIT MANAGER
                    //cmd.CommandText = "SELECT wa.fullname AS fullname FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.zonecode WHERE wa.zonecode=@ram_zonecode AND wb.class_04 =@ram_class_04 AND wb.region=@ram_region AND wb.areacode=@ram_areacode AND wa.job_title = 'RAM'";
                    cmd.CommandText = "SELECT TOP 1 wa.fullname AS fullname, wa.res_id as resId FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.zonecode WHERE wa.zonecode=@ram_zonecode AND wb.class_03 =@ram_class_03 AND wb.class_04 =@ram_class_04 AND wb.region=@ram_region AND wb.areacode=@ram_areacode AND wa.job_title = 'RAM'";

                    cmd.Parameters.AddWithValue("@ram_zonecode", zonecode);
                    cmd.Parameters.AddWithValue("@ram_class_04", class_04);
                    cmd.Parameters.AddWithValue("@ram_class_03", region);
                    cmd.Parameters.AddWithValue("@ram_region", region);
                    cmd.Parameters.AddWithValue("@ram_areacode", areacode);

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            ramCred.Add(rdr["fullname"].ToString().Trim());
                            ramCred.Add(rdr["resId"].ToString().Trim());


                        }
                        else
                        {
                            //approvers.Add(string.Empty);
                            ramCred.Add(string.Empty);
                            ramCred.Add(string.Empty);

                        }
                        con.Close();
                        rdr.Close();
                    }
                }
            }
            return ramCred;
        }
        #endregion

        #region ASST VPO
        private List<string> GetGmoAstGenman(string zonecode)
        {
            //var asstGenManFullname = "";
            var asstGenManCred = new List<string>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //GET ASST GENMAN
                    if (zonecode == "VISAYAS" || zonecode == "MINDANAO" || zonecode == "VISMIN")
                    {

                        //GET GMO-ASSTGENMAN || ASST.VPO
                        var task = "GMO-HELPDESK";
                        var zone = "VISMIN";
                        var resId = "1013127";

                        cmd.CommandText = "SELECT wa.fullname AS fullname, wa.res_id as resId FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.Zonecode WHERE wa.zonecode=@zone AND wa.task = @task AND res_id=@resId";
                        cmd.Parameters.AddWithValue("@task", task);
                        cmd.Parameters.AddWithValue("@zone", zone);
                        cmd.Parameters.AddWithValue("@resId", resId);

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();

                                asstGenManCred.Add(rdr["fullname"].ToString().Trim());
                                asstGenManCred.Add(rdr["resId"].ToString().Trim());

                            }
                            else
                            {
                                asstGenManCred.Add(string.Empty);
                                asstGenManCred.Add(string.Empty);

                            }
                            con.Close();
                            rdr.Close();
                        }
                    }
                    else
                    {
                        cmd.CommandText = "SELECT wa.fullname AS fullname, wa.res_id as resId FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.Zonecode WHERE wa.zonecode='LUZON' AND wa.job_title = 'GMO-ASTGENMAN' OR wa.task = 'GMO-ASTGENMAN' AND res_id='19930045'";

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                asstGenManCred.Add(rdr["fullname"].ToString().Trim());
                                asstGenManCred.Add(rdr["resId"].ToString().Trim());


                            }
                            else
                            {
                                asstGenManCred.Add(string.Empty);
                                asstGenManCred.Add(string.Empty);

                            }
                            con.Close();
                            rdr.Close();
                        }
                    }
                }
            }
            return asstGenManCred;
        }
        #endregion

        #region VPO
        private List<string> GetGmoGenman(string zonecode)
        {
            //var GenManFullname = "";

            var GenManCred = new List<string>();


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {

                    if (zonecode == "VISAYAS" || zonecode == "MINDANAO" || zonecode == "VISMIN")
                    {
                        //VISMIN
                        //GET GMO-GENMAN || VPO

                        cmd.CommandText = "SELECT wa.fullname AS fullname, wa.res_id as resId FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.Zonecode WHERE wa.zonecode='VISMIN' AND wa.job_title = 'GMO-GENMAN' OR wa.task = 'GMO-GENMAN' AND res_id='94002722'";

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                //approvers.Add(rdr["fullname"].ToString().Trim());
                                GenManCred.Add(rdr["fullname"].ToString().Trim());
                                GenManCred.Add(rdr["resId"].ToString().Trim());

                            }
                            else
                            {
                                GenManCred.Add(string.Empty);
                                GenManCred.Add(string.Empty);
                            }
                            con.Close();
                            rdr.Close();
                        }

                    }
                    else
                    {

                        //LUZON
                        //GET GMO-GENMAN || VPO

                        cmd.CommandText = "SELECT wa.fullname AS fullname, wa.res_id as resId FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.Zonecode WHERE wa.zonecode='LUZON' AND wa.job_title = 'GMO-GENMAN' OR wa.task = 'GMO-GENMAN' AND res_id='19890004'";

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                GenManCred.Add(rdr["fullname"].ToString().Trim());
                                GenManCred.Add(rdr["resId"].ToString().Trim());

                            }
                            else
                            {
                                GenManCred.Add(string.Empty);
                                GenManCred.Add(string.Empty);

                            }
                            con.Close();
                            rdr.Close();
                        }

                    }

                }
            }
            return GenManCred;
        }
        #endregion

    }
}