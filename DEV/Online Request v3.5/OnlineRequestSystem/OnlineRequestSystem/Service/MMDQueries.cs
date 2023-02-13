using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OnlineRequestSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web.Mvc;

namespace OnlineRequestSystem.Service
{
    public class MMDQueries
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MMDQueries));
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;
        private string format = "yyyy/MM/dd HH:mm:ss";

        public string GetDivisionName(string DivCode)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT DivName FROM OnlineRequest.division WHERE DivID = @DivCode";
                cmd.Parameters.AddWithValue("@DivCode", DivCode);
                conn.Open();
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    rdr.Read();
                    if (rdr.HasRows) { return rdr["DivName"].ToString().Trim(); }
                    else { return ""; }
                }

            }
        }

        public List<SelectListItem> ListOfDivision(string zonecode)
        {
            try
            {
                var ListItem = new List<SelectListItem>();
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.Parameters.AddWithValue("@zcode", zonecode);
                    cmd.CommandText = "SELECT * FROM OnlineRequest.division WHERE ZoneCode = @zcode";
                    conn.Open();
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ListItem.Add(new SelectListItem { Text = rdr["DivName"].ToString().Trim(), Value = rdr["DivID"].ToString().Trim() });
                    }
                }
                return ListItem;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }

        public List<SelectListItem> Get_IRDivisions(string zonecode)
        {
            try
            {
                var xlist = new List<SelectListItem>();
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                Uri source = new System.Uri(w + "/GetListOfDivisions");
                RequestHandler reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return null;

                var resData = JsonConvert.DeserializeObject<ListOfDivisionResponse>(x);
                foreach (var item in resData.ListOfDivisions)
                {
                    xlist.Add(new SelectListItem { Text = item.DivisionEscalation, Value = item.DivisionEscalation });
                }
                return xlist;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }

        public List<SelectListItem> Get_OnlineRequestDivisions(string zonecode)
        {
            var ListItem = new List<SelectListItem>();
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.Parameters.AddWithValue("@zcode", zonecode);
                cmd.CommandText = "SELECT * FROM division WHERE ZoneCode = @zcode ORDER BY DivName";
                conn.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ListItem.Add(new SelectListItem { Text = rdr["DivName"].ToString().Trim(), Value = rdr["DivName"].ToString().Trim() });
                }
            }
            return ListItem;
        }

        public string GetDivisionAcro(string divName)
        {
            string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
            Uri source = new System.Uri(w + "/GetDivisionAcro?divName=" + divName);
            RequestHandler reqHandler = new RequestHandler(source, "GET", "application/json");
            string response = reqHandler.HttpGetRequest();
            string ee = response.Substring(0, 5);
            if (ee == "Error")
                log.Fatal("No Service Available.");

            var Response = JsonConvert.DeserializeObject<DivisionAcroResponses>(response);
            return Response.DivisionAcro;
        }

        #region Get Division Details 
        public DivisionDetails GetDivisionDetails(string DivAcro)
        {
            string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
            Uri source = new System.Uri(w + "/GetDivisionDetails?DivAcro=" + DivAcro);
            RequestHandler reqHandler = new RequestHandler(source, "GET", "application/json");
            string response = reqHandler.HttpGetRequest();
            string ee = response.Substring(0, 5);
            if (ee == "Error")
                log.Fatal("No Service Available.");

            var Response = JsonConvert.DeserializeObject<DivisionDetailsResponses>(response);
            return Response.DivisionDetailsResp;
        }
        #endregion Get Division Details

        #region Get Division Approver Resource ID
        public string GetDivisionApproverResourceID(string zonecode, string DivisionManager)
        {
            string Result = "";
            try
            {
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                Uri source = new System.Uri(w + "/GetDivisionApproverResourceID?zonecode=" + zonecode + "&DivisionManager=" + DivisionManager);
                RequestHandler reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return "Service Unavailable";

                var Response = JsonConvert.DeserializeObject<DivisionApproverResourceIDResponse>(x);
                return Response.DivisionApproverResouce;
            }
            catch (Exception x)
            {
                log.Error(x.Message);
                return Result;
            }
        }
        #endregion Get Division Approver Resource ID

        #region Get Department List
        public DepartmentInfo GetDepartmentList(string zoneCode)
        {
            var dinfo = new DepartmentInfo();
            var db = new ORtoMySql();
            var dlist = new List<DepartmentInfo>();
            using (var conn = db.getConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.Parameters.AddWithValue("@ZoneCode", zoneCode);
                cmd.CommandText = "SELECT * FROM OnlineRequest.department Where ZoneCode = @ZoneCode";
                conn.Open();
                using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        var x = new DepartmentInfo();
                        x.deptID = rdr["id"].ToString().Trim();
                        x.deptManager = rdr["deptManager"].ToString().Trim();
                        x.approver_resID = rdr["approver_resID"].ToString().Trim();
                        x.division = rdr["division"].ToString().Trim();
                        x.ZoneCode = rdr["ZoneCode"].ToString().Trim();
                        dlist.Add(x);
                    }
                }
                dinfo._info = dlist;
                return dinfo;
            }
        }
        #endregion Get Department List

        #region Add new Department

        public string AddNewDepartment(DepartmentInfo data, ORSession ss)
        {
            string Resp = "";
            try
            {
                var db = new ORtoMySql();
                object divecodeResult = "";
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();

                    string Query = " INSERT INTO OnlineRequest.department (deptCode, deptManager, approver_resID, division, divAcro, ZoneCode , divcode , CostCenter,  syscreated,syscreator) VALUES " +
                                   " (@deptCode, @deptManager, @approver_resID, @division, @divAcro, @zonecode,@divcode, @costCenter, @syscreated, @syscreator)";
                    cmd.Parameters.AddWithValue("@deptCode", data.deptCode);
                    cmd.Parameters.AddWithValue("@deptManager", data.deptManager.Trim());
                    cmd.Parameters.AddWithValue("@approver_resID", data.approver_resID.Trim());
                    cmd.Parameters.AddWithValue("@division", data.division);
                    cmd.Parameters.AddWithValue("@divAcro", data.divacro);
                    cmd.Parameters.AddWithValue("@zonecode", ss.s_zonecode);
                    cmd.Parameters.AddWithValue("@divcode", data.divcode);
                    cmd.Parameters.AddWithValue("@costCenter", data.deptCode);
                    cmd.Parameters.AddWithValue("@syscreated", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@syscreator", ss.s_fullname);
                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();
                    cmd.CommandTimeout = 0;
                }
                Resp = "success";
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                Resp = "error";
            }
            return Resp;
        }

        #endregion Add new Department

        #region Update Department

        public string UpdateDepartment(DepartmentInfo data, ORSession ss)
        {
            string Resp = "";
            try
            {
                var db = new ORtoMySql();
                object divecodeResult = "";
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    string Query = "UPDATE OnlineRequest.department SET  deptManager = @deptManager , approver_resID = @approver_resID , division = @division , divAcro = @divAcro , ZoneCode = @zonecode, divcode = @divcode, sysmodified = @sysmodified, sysmodifier = @sysmodifier WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", data.deptID.Trim());
                    cmd.Parameters.AddWithValue("@deptManager", data.deptManager.Trim());
                    cmd.Parameters.AddWithValue("@approver_resID", data.approver_resID.Trim());
                    cmd.Parameters.AddWithValue("@division", data.division);
                    cmd.Parameters.AddWithValue("@divAcro", data.divacro);
                    cmd.Parameters.AddWithValue("@zonecode", ss.s_zonecode);
                    cmd.Parameters.AddWithValue("@divcode", data.divcode);
                    cmd.Parameters.AddWithValue("@sysmodified", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@sysmodifier", ss.s_fullname);
                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();
                    cmd.CommandTimeout = 0;
                }
                Resp = "success";
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                Resp = "error";
            }
            return Resp;
        }

        #endregion Update Department

        #region Checking Divcode during adding Division

        public bool CheckingDivCode(string DivAcro, string DivCode)
        {
            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        string CheckDiv;
                        CheckDiv = "SELECT * FROM OnlineRequest.division WHERE DivAcro = @DivAcro and DivCode = @DivCode";
                        cmd.Parameters.AddWithValue("@DivAcro", DivAcro);
                        cmd.Parameters.AddWithValue("@DivCode", DivCode);
                        cmd.CommandText = CheckDiv;
                        var read = cmd.ExecuteReader();
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
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }

        #endregion Checking Divcode during adding Division

        #region Get IR Div Code

        public object getIRDivCode(string DivCode)
        {
            string Result = "";
            try
            {
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                Uri source = new System.Uri(w + "/GetIRDivCode?DivCode=" + DivCode);
                RequestHandler reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                {
                    return "Service Unavailable";
                }
                var resData = JsonConvert.DeserializeObject<IRDivCodeResponse>(x);
                Result = resData.IRDivCode;
            }
            catch (Exception)
            {
                return Result;
            }
            return Result;
        }

        #endregion Get IR Div Code

        #region Get Div ID
        public string Get_DivID(string divAcro)
        {
            int DivID = 0;
            string returnValue = "";
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    String x = "SELECT MAX(SUBSTR(DivID, LENGTH(DivID) -3, LENGTH(DivID))) AS DivID FROM OnlineRequest.division";
                    cmd.CommandText = x;
                    cmd.CommandTimeout = 0;

                    using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (read.HasRows)
                        {
                            if (read.Read())
                            {
                                var fromdb = read["DivID"].ToString().Trim();
                                if (fromdb == "")
                                {
                                    DivID = 00001;
                                }
                                else
                                {
                                    DivID = Convert.ToInt32(fromdb) + 1;
                                }
                                returnValue = "DIV" + divAcro + DivID.ToString().PadLeft(4, '0');
                            }
                        }
                        return returnValue;
                    }
                }
            }
        }
        #endregion Get Div ID

        #region Get Division Cost Center 
        internal string GetDivisionCostCenter(string divacro)
        {
            string Result = "";
            try
            {
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                Uri source = new System.Uri(w + "/GetDivisionCostCenter?divacro=" + divacro);
                RequestHandler reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                {
                    return "Service Unavailable";
                }
                var resData = JsonConvert.DeserializeObject<DivisionCostCenterResponse>(x);
                Result = resData.DivisionCostCenter;
            }
            catch (Exception)
            {
                return Result;
            }
            return Result;
        }
        #endregion Get Division Cost Center

        #region Get Region of the Request
        internal string GetRegionOfTheRequest(string ReqNo)
        {
            string returnValue = "";
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();

                String x = "Select Region from onlineRequest_Open where reqNumber = @reqNumber";
                cmd.Parameters.AddWithValue("@reqNumber", ReqNo);
                cmd.CommandText = x;
                var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (rdr.HasRows)
                {
                    rdr.Read();
                    returnValue = rdr["Region"].ToString().Trim();
                }
                else
                {
                    returnValue = "No region detected";
                }
            }
            return returnValue;
        }
        #endregion Get Region of the Request

        internal bool Check_DivApproverResID(string resid, string ZoneCode)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    String x = "SELECT ApproverResID FROM division WHERE ApproverResID = @resid and ZoneCode = @zoneCode";
                    cmd.Parameters.AddWithValue("@zoneCode", ZoneCode);
                    cmd.Parameters.AddWithValue("@resid", resid);
                    cmd.CommandText = x;
                    cmd.CommandTimeout = 0;
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
        }

        internal bool Check_DeptApproverResID(string resid, string ZoneCode)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    String x = "SELECT approver_resID FROM department WHERE approver_resID = @resid and ZoneCode = @zcode";
                    cmd.Parameters.AddWithValue("@zcode", ZoneCode);
                    cmd.Parameters.AddWithValue("@resid", resid);
                    cmd.CommandText = x;
                    cmd.CommandTimeout = 0;
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
        }

        public VPassistantInfo GetVPAssistants()
        {
            var DInfo = new VPassistantInfo();
            var db = new ORtoMySql();
            var VPs = new List<VPassistantInfo>();
            using (var conn = db.getConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    string Query = "SELECT * FROM OnlineRequest.VPassistant";
                    cmd.CommandText = Query;
                    cmd.CommandTimeout = 0;
                    using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (read.Read())
                        {
                            var x = new VPassistantInfo();
                            x.vp_id = Convert.ToInt32(read["id"]);
                            x.VPAssistant = read["VPAssistant"].ToString().Trim();
                            x.Division = read["Division"].ToString().Trim();
                            x.UserID = read["approver_resid"].ToString().Trim();
                            x.Zonecode = read["Zonecode"].ToString().Trim();
                            VPs.Add(x);
                        }
                    }
                    DInfo._info = VPs;
                    return DInfo;
                }
            }
        }

        internal string UpdateVPAssistant(VPassistantInfo info)
        {
            string Resp = "";
            try
            {
                var db = new ORtoMySql();
                object divecodeResult = "";
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    string Query = "UPDATE OnlineRequest.VPassistant SET  VPAssistant = @vp , Division = @div , Zonecode = @zcode, approver_resid = @apprv WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", info.vp_id);
                    cmd.Parameters.AddWithValue("@vp", info.VPAssistant);
                    cmd.Parameters.AddWithValue("@div", info.Division);
                    cmd.Parameters.AddWithValue("@zcode", info.Zonecode);
                    cmd.Parameters.AddWithValue("@apprv", info.UserID);
                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();
                    cmd.CommandTimeout = 0;
                }
                Resp = "success";
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                Resp = "error";
            }
            return Resp;
        }

        //For Item Pricing: Search Item Price
        //public ItemsInfo SearchItemPrice(string searchCriteria)
        //{
        //    string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
        //    Uri source = new System.Uri(w + "/SearchItem?itemDetails=" + searchCriteria);
        //    RequestHandler reqHandler = new RequestHandler(source, "GET", "application/json");
        //    string response = reqHandler.HttpGetRequest();
        //    string ee = response.Substring(0, 5);
        //    if (ee == "Error")
        //        log.Fatal("No Service Available");

        //    var Response = JsonConvert.DeserializeObject<ItemsInfoResponses>(response);
        //    return Response.ItemsInfoResp;
        //}

    }
}