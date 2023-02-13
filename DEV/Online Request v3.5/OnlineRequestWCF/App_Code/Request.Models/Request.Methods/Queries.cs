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

        #region CASH REQUEST

        protected internal CashRequestApproversResponse CashRequestApprovers(string zonecode, string class_04, string region, string areacode, string task)
        {
            try
            {
                var ApproverList = new CashRequestApproversResponse();
                ApproverList.ListOfCashRequestApprovers = new List<ListOfCashRequestApprovers>();

                List<string> approvers = new List<string>();


                _log.Info(zonecode + class_04 + region + areacode);

                string am = GetAreaManager(zonecode, class_04, region, areacode);
                string rm = GetRManager(zonecode, class_04, region, areacode);
                string ram = GetRAManager(zonecode, class_04, region, areacode);


                if (task == "BM" || task == "ABM" || task == "TELLER")
                {

                    if (am == "" || rm == "" || ram == "")
                    {
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                    }
                    else
                    {
                        approvers.Add(GetAreaManager(zonecode, class_04, region, areacode));
                        approvers.Add(GetRManager(zonecode, class_04, region, areacode));
                        approvers.Add(GetRAManager(zonecode, class_04, region, areacode));
                        approvers.Add(GetGmoAstGenman(zonecode));
                        approvers.Add(GetGmoGenman(zonecode));
                    }

                }
                else if (task == "AREA MANAGER")
                {

                    if (am == "" || rm == "" || ram == "")
                    {
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                        approvers.Add(string.Empty);
                    }
                    else
                    {
                        approvers.Add(string.Empty);
                        approvers.Add(GetRManager(zonecode, class_04, region, areacode));
                        approvers.Add(GetRAManager(zonecode, class_04, region, areacode));
                        approvers.Add(GetGmoAstGenman(zonecode));
                        approvers.Add(GetGmoGenman(zonecode));
                    }


                }
                else if (task == "REGIONAL MAN")
                {

                    if (am == "" || rm == "" || ram == "")
                    {
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
                        approvers.Add(GetRAManager(zonecode, class_04, region, areacode));
                        approvers.Add(GetGmoAstGenman(zonecode));
                        approvers.Add(GetGmoGenman(zonecode));
                    }


                }
                else if (task == "RAM")
                {
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(GetGmoAstGenman(zonecode));
                    approvers.Add(GetGmoGenman(zonecode));
                }
                else if (task == "GMO-ASTGENMAN")
                {
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(GetGmoGenman(zonecode));
                }
                else if (task == "GMO-GENMAN")
                {
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                    approvers.Add(string.Empty);
                }

                ApproverList.ListOfCashRequestApprovers.Add(new ListOfCashRequestApprovers
                {
                    AmName = approvers[0],
                    RmName = approvers[1],
                    RamName = approvers[2],
                    GmoGenAsstName = approvers[3],
                    GmoGenName = approvers[4]
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
        private string GetAreaManager(string zonecode, string class_04, string region, string areacode)
        {
            var amFullname = "";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //GET AREA MANAGER
                    cmd.CommandText = "SELECT wa.fullname AS fullname FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.zonecode WHERE wa.zonecode=@zonecode AND wb.class_04 =@class_04 AND wb.region=@region AND wb.areacode=@areacode AND wa.task = 'AREA MANAGER'";

                    cmd.Parameters.AddWithValue("@zonecode", zonecode);
                    cmd.Parameters.AddWithValue("@class_04", class_04);
                    cmd.Parameters.AddWithValue("@region", region);
                    cmd.Parameters.AddWithValue("@areacode", areacode);

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            //approvers.Add(rdr["fullname"].ToString().Trim());
                            amFullname = rdr["fullname"].ToString().Trim();

                        }
                        else
                        {
                            //approvers.Add(string.Empty);
                            amFullname = string.Empty;

                        }
                        con.Close();
                        rdr.Close();
                    }
                }
            }
            return amFullname;
        }
        #endregion

        #region REGIONAL MANAGER
        private string GetRManager(string zonecode, string class_04, string region, string areacode)
        {
            var rmFullname = "";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //GET REGIONAL MANAGER
                    cmd.CommandText = "SELECT wa.fullname AS fullname FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.zonecode WHERE wa.zonecode=@rm_zonecode AND wb.class_04 =@rm_class_04 AND wb.region=@rm_region AND wb.areacode=@rm_areacode AND wa.task = 'REGIONAL MAN'";

                    cmd.Parameters.AddWithValue("@rm_zonecode", zonecode);
                    cmd.Parameters.AddWithValue("@rm_class_04", class_04);
                    cmd.Parameters.AddWithValue("@rm_region", region);
                    cmd.Parameters.AddWithValue("@rm_areacode", areacode);

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            //approvers.Add(rdr["fullname"].ToString().Trim());
                            rmFullname = rdr["fullname"].ToString().Trim();

                        }
                        else
                        {
                            //approvers.Add(string.Empty);
                            rmFullname = string.Empty;

                        }
                        con.Close();
                        rdr.Close();
                    }
                }
            }
            return rmFullname;
        }
        #endregion

        #region REGIONAL AUDIT MANAGER
        private string GetRAManager(string zonecode, string class_04, string region, string areacode)
        {
            var ramFullname = "";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //GET REGIONAL AUDIT MANAGER
                    cmd.CommandText = "SELECT wa.fullname AS fullname FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.zonecode WHERE wa.zonecode=@ram_zonecode AND wb.class_04 =@ram_class_04 AND wb.region=@ram_region AND wb.areacode=@ram_areacode AND wa.task = 'RAM'";

                    cmd.Parameters.AddWithValue("@ram_zonecode", zonecode);
                    cmd.Parameters.AddWithValue("@ram_class_04", class_04);
                    cmd.Parameters.AddWithValue("@ram_region", region);
                    cmd.Parameters.AddWithValue("@ram_areacode", areacode);

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            //approvers.Add(rdr["fullname"].ToString().Trim());
                            ramFullname = rdr["fullname"].ToString().Trim();

                        }
                        else
                        {
                            //approvers.Add(string.Empty);
                            ramFullname = string.Empty;

                        }
                        con.Close();
                        rdr.Close();
                    }
                }
            }
            return ramFullname;
        }
        #endregion

        #region ASST VPO
        private string GetGmoAstGenman(string zonecode)
        {
            var asstGenManFullname = "";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //GET ASST GENMAN
                    if (zonecode == "VISAYAS" || zonecode == "MINDANAO" || zonecode == "VISMIN")
                    {

                        //GET GMO-ASSTGENMAN || ASST.VPO

                        cmd.CommandText = "SELECT wa.fullname AS fullname FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.Zonecode WHERE wa.zonecode='VISMIN' AND wa.task = 'GMO-ASTGENMAN' AND res_id='94016508'";

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                //approvers.Add(rdr["fullname"].ToString().Trim());
                                asstGenManFullname = rdr["fullname"].ToString().Trim();

                            }
                            else
                            {
                                //approvers.Add(string.Empty);
                                asstGenManFullname = string.Empty;

                            }
                            con.Close();
                            rdr.Close();
                        }
                    }
                    else
                    {
                        cmd.CommandText = "SELECT wa.fullname AS fullname FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.Zonecode WHERE wa.zonecode='LUZON' AND wa.task = 'GMO-ASTGENMAN' AND res_id='19930045'";

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                //approvers.Add(rdr["fullname"].ToString().Trim());
                                asstGenManFullname = rdr["fullname"].ToString().Trim();


                            }
                            else
                            {
                                //approvers.Add(string.Empty);
                                asstGenManFullname = string.Empty;

                            }
                            con.Close();
                            rdr.Close();
                        }
                    }
                }
            }
            return asstGenManFullname;
        }
        #endregion

        #region VPO
        private string GetGmoGenman(string zonecode)
        {
            var GenManFullname = "";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebProject"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {

                    if (zonecode == "VISAYAS" || zonecode == "MINDANAO" || zonecode == "VISMIN")
                    {
                        //VISMIN
                        //GET GMO-GENMAN || VPO

                        cmd.CommandText = "SELECT wa.fullname AS fullname FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.Zonecode WHERE wa.zonecode='VISMIN' AND wa.task = 'GMO-GENMAN' AND res_id='94002722'";

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                //approvers.Add(rdr["fullname"].ToString().Trim());

                                GenManFullname = rdr["fullname"].ToString().Trim();

                            }
                            else
                            {
                                //approvers.Add(string.Empty);
                                GenManFullname = string.Empty;
                            }
                            con.Close();
                            rdr.Close();
                        }

                    }
                    else
                    {

                        //LUZON
                        //GET GMO-GENMAN || VPO

                        cmd.CommandText = "SELECT wa.fullname AS fullname FROM WebAccounts wa INNER JOIN WebBranches wb ON wa.comp=wb.bedrnr AND wa.zonecode=wb.Zonecode WHERE wa.zonecode='LUZON' AND wa.task = 'GMO-GENMAN' AND res_id='19890004'";

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                //approvers.Add(rdr["fullname"].ToString().Trim());
                                GenManFullname = rdr["fullname"].ToString().Trim();

                            }
                            else
                            {
                                //approvers.Add(string.Empty);
                                GenManFullname = string.Empty;

                            }
                            con.Close();
                            rdr.Close();
                        }

                    }

                }
            }
            return GenManFullname;
        }
        #endregion

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
    }
}