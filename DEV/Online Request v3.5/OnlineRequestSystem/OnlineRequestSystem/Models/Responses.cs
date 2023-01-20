using System.Collections.Generic;

namespace OnlineRequestSystem.Models
{
    public class LoginResponse : Resp
    {
        private ORSession _logdata;

        public ORSession logdata
        {
            get { return _logdata; }
            set { _logdata = value; }
        }
    }

    public class BranchNameResponse : Resp
    {
        private string _branchName;

        public string BranchName
        {
            get { return _branchName; }
            set { _branchName = value; }
        }
    }

    public class DivisionNameResponse : Resp
    {
        private string _divisionName;

        public string DivisionName
        {
            get { return _divisionName; }
            set { _divisionName = value; }
        }
    }

    public class BranchCodeResponse : Resp
    {
        private string _branchCode;

        public string BranchCode
        {
            get { return _branchCode; }
            set { _branchCode = value; }
        }
    }

    public class DivisionCostCenterResponse : Resp
    {
        private string _divisionCostcenter;

        public string DivisionCostCenter
        {
            get { return _divisionCostcenter; }
            set { _divisionCostcenter = value; }
        }
    }

    public class IRDivCodeResponse : Resp
    {
        private string _IRDivCode;

        public string IRDivCode
        {
            get { return _IRDivCode; }
            set { _IRDivCode = value; }
        }
    }

    public class DivisionApproverResourceIDResponse : Resp
    {
        private string _DivAppResID;

        public string DivisionApproverResouce
        {
            get { return _DivAppResID; }
            set { _DivAppResID = value; }
        }
    }

    public class DivisionDetailsResponses : Resp
    {
        private DivisionDetails _DivisionDetails;

        public DivisionDetails DivisionDetailsResp
        {
            get { return _DivisionDetails; }
            set { _DivisionDetails = value; }
        }
    }

    public class DivisionAcroResponses : Resp
    {
        private string _DivisionAco;

        public string DivisionAcro
        {
            get { return _DivisionAco; }
            set { _DivisionAco = value; }
        }
    }

    public class ListOfBranchesResponse : Resp
    {
        public List<ListOfBranches> ListOfBranches { get; set; }
    }

    public class ListOfBranches
    {
        public string BranchName { get; set; }
        public string BranchNameValue { get; set; }
    }

    public class ListOfDivisionResponse : Resp
    {
        public List<ListOfDivisions> ListOfDivisions { get; set; }
    }

    public class ListOfDivisions
    {
        public string DivisionEscalation { get; set; }
        public string DivisionCode { get; set; }
    }

    public class ListOfIRDivisionResponse : Resp
    {
        public List<ListOfIRDivisions> ListIRDivisions { get; set; }
    }

    public class ListOfIRDivisions
    {
        public string DivisionName { get; set; }
        public string CostCenter { get; set; }
    }

    public class ListOfAreaResponse : Resp
    {
        public List<ListOfAreas> ListOfAreas { get; set; }
    }

    public class ListOfAreas
    {
        public string Area { get; set; }
        public string AreaValue { get; set; }
    }

    public class ListOfRegionResponse : Resp
    {
        public List<ListOfRegions> ListOfRegions { get; set; }
    }

    public class ListOfRegions
    {
        public string Region { get; set; }
        public string RegionValue { get; set; }
    }
}