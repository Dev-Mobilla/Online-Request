using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace OnlineRequestWCF.Request.Models
{      
    [DataContract]
    public class LoginResponse : Responses
    {
        private UserInformation _userdata;
        [DataMember]
        public UserInformation logdata
        {
            get { return _userdata; }
            set { _userdata = value; }
        }
    }

    [DataContract]
    public class UserInformation
    {
        private string fullname; private string usr_id; private string res_id;
        private string comp; private string costcenter; private string zonecode;
        private string division; private string regioncode; private int login;
        private string divcode; private string divacro; private string divmanager;
        private string departmanager; private string job_title; private string task;
        private string blocked; private string bedrnm; private string area; private string region;
        private string areacode; private string BMname; private string AMname; private string LPTLname; private string RMname;
        [DataMember]
        public string s_fullname { get { return fullname; } set { fullname = value; } }
        [DataMember]
        public string s_usr_id { get { return usr_id; } set { usr_id = value; } }
        [DataMember]
        public string s_res_id { get { return res_id; } set { res_id = value; } }
        [DataMember]
        public string s_comp { get { return comp; } set { comp = value; } }
        [DataMember]
        public string s_costcenter { get { return costcenter; } set { costcenter = value; } }
        [DataMember]
        public string s_zonecode { get { return zonecode; } set { zonecode = value; } }
        [DataMember]
        public string s_Division { get { return division; } set { division = value; } }
        [DataMember] 
        public string s_DivCode { get { return divcode; } set { divcode = value; } }
        [DataMember]
        public string s_DivAcro { get { return divacro; } set { divacro = value; } }
        [DataMember]
        public string s_DivManager { get { return divmanager; } set { divmanager = value; } }
        [DataMember]
        public string s_DepartManager { get { return departmanager; } set { departmanager = value; } }
        [DataMember]
        public string s_job_title { get { return job_title; } set { job_title = value; } }
        [DataMember]
        public string s_task { get { return task; } set { task = value; } }
        [DataMember]
        public string s_blocked { get { return blocked; } set { blocked = value; } }
        [DataMember]
        public string s_bedrnm { get { return bedrnm; } set { bedrnm = value; } }
        [DataMember]
        public string s_area { get { return area; } set { area = value; } }
        [DataMember]
        public string s_region { get { return region; } set { region = value; } }
        [DataMember]
        public string s_regionCode { get { return regioncode; } set { regioncode = value; } }
        [DataMember]
        public string s_areaCode { get { return areacode; } set { areacode = value; } }
        [DataMember]
        public string s_BMName { get { return BMname; } set { BMname = value; } }
        [DataMember]
        public string s_AMName { get { return AMname; } set { AMname = value; } }
        [DataMember]
        public string s_LPTLName { get { return LPTLname; } set { LPTLname = value; } }
        [DataMember]
        public string s_RMName { get { return RMname; } set { RMname = value; } }
        [DataMember]
        public int s_login { get { return login; } set { login = value; } }
    }
}