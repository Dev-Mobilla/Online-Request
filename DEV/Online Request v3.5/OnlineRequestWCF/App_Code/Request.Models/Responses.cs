using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace OnlineRequestWCF.Request.Models
{
    [DataContract]
    public class Responses
    {
        [DataMember]
        public string resCode { get; set; }
        [DataMember]
        public string resMsg { get; set; }
    }

    [DataContract]
    public class BranchNameResponse : Responses
    {
        [DataMember]
        public string BranchName { get; set; }
    }

    [DataContract]
    public class DivisionNameResponse : Responses
    {
        [DataMember]
        public string DivisionName { get; set; }
    }

    [DataContract]
    public class BranchCodeResponses : Responses
    {
        [DataMember]
        public string BranchCode { get; set; }
    }

    [DataContract]
    public class DivisionCostCenterResponses : Responses
    {
        [DataMember]
        public string DivisionCostCenter { get; set; }
    }

    [DataContract]
    public class IRDivCodeResponses : Responses
    {
        [DataMember]
        public string IRDivCode { get; set; }
    }   

    [DataContract]
    public class DivManResourceIDResponses : Responses
    {
        [DataMember]
        public string DivisionApproverResouce { get; set; }
    }

    [DataContract]
    public class DivisionDetailsResponses : Responses
    {
        private DivisionDetails _DivisionDetails;
        [DataMember]
        public DivisionDetails DivisionDetailsResp
        {
            get { return _DivisionDetails; }
            set { _DivisionDetails = value; }
        }
    }

    [DataContract]
    public class DivisionAcroResponses : Responses 
    {
        [DataMember]
        public string DivisionAcro { get; set; }
    }

    [DataContract]
    public class DivisionDetails
    {
        #region Private Variables
        private string _DivID;  
        private string _DivisionEscalation;
        private string _Division;
        private string _DivisionCode;
        private string _DivisionAcro; 
        private string _ZoneCode;
        private string _CostCenter; 
        private string _DivisionManager;
        private string _DivisionResourceID; 
        private string _ContactNum;
        #endregion

        [DataMember]
        public string DivID
        {
            get { return _DivID; }
            set { _DivID = value; }
        }
        [DataMember]
        public string DivisionEscalation
        {
            get { return _DivisionEscalation; }
            set { _DivisionEscalation = value; }
        }
        [DataMember]
        public string Division
        {
            get { return _Division; }
            set { _Division = value; }
        }
        [DataMember]
        public string DivisionCode 
        {
            get { return _DivisionCode; }
            set { _DivisionCode = value; }
        }
        [DataMember]
        public string DivisionAcro
        {
            get { return _DivisionAcro; }
            set { _DivisionAcro = value; }
        }
        [DataMember]
        public string ZoneCode 
        {
            get { return _ZoneCode; }
            set { _ZoneCode = value; }
        }
        [DataMember]
        public string CostCenter 
        {
            get { return _CostCenter; }
            set { _CostCenter = value; }
        }
        [DataMember]
        public string DivisionManager 
        {
            get { return _DivisionManager; }
            set { _DivisionManager = value; }
        }
        [DataMember]
        public string DivisionResourceID 
        {
            get { return _DivisionResourceID; }
            set { _DivisionResourceID = value; }
        }
        [DataMember]
        public string ContactNum 
        {
            get { return _ContactNum; }
            set { _ContactNum = value; }
        }
    }


    [DataContract]
    public class ListOfBranchesResponse :Responses
    {
        [DataMember]
        public List<ListOfBranches> ListOfBranches { get; set;}
    }

    public class ListOfBranches 
    {
        public string BranchName { get; set; }
        public string BranchNameValue { get; set; }
    }

    [DataContract]
    public class ListOfDivisionResponse : Responses 
    {
        [DataMember]
        public List<ListOfDivisions> ListOfDivisions { get; set; }
    }
    public class ListOfDivisions 
    {
        public string DivisionEscalation { get; set; }
        public string DivisionCode { get; set; }
    }

    [DataContract]
    public class ListOfAreaResponse : Responses
    {
        [DataMember]
        public List<ListOfAreas> ListOfAreas { get; set; }
    }
    public class ListOfAreas
    {
        public string Area { get; set; }
        public string AreaValue { get; set; }
    }

    [DataContract]
    public class ListOfRegionResponse : Responses 
    {
        [DataMember]
        public List<ListOfRegions> ListOfRegions { get; set; }
    }
    public class ListOfRegions
    {
       public string Region { get; set; }
       public string RegionValue { get; set; }
    }

    [DataContract]
    public class ListOfSDCResponse : Responses 
    {
        [DataMember]
        public List<SDCDetails> ListOfSDC { get; set; }
    }
    public class SDCDetails 
    {
        public string Region { get; set; }
        public string RM { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ZoneCode { get; set; }   
    }

    [DataContract]
    public class ListOfDivisionManagerResponse : Responses 
    {
        [DataMember]
        public List<ListOfDivisionManagers> ListOfDivisionManagers { get; set; }
    }

    public class ListOfDivisionManagers 
    {
        public string DivMan { get; set; }
        public string DivManValue { get; set; }
    }

    [DataContract]
    public class ListOfIRDivisionResponse : Responses 
    {
        [DataMember]
        public List<ListOfIRDivisions> ListIRDivisions { get; set; }
    }
    public class ListOfIRDivisions 
    {
        public string DivisionName { get; set;}
        public string CostCenter { get; set; }
    }
    #region CASH REQUEST

    [DataContract]
    public class CashRequestApproversResponse : Responses
    {
        [DataMember]
        public List<ListOfCashRequestApprovers> ListOfCashRequestApprovers { get; set; }

    }


    public class ListOfCashRequestApprovers
    {
        #region Private Variables Cash Request

        private string _AmName;
        private string _RmName;
        private string _RamName;
        private string _GmoGenAsstName;
        private string _GmoGenName;

        #endregion

        [DataMember]
        public string AmName
        {
            get { return _AmName; }
            set { _AmName = value; }
        }
        [DataMember]
        public string RmName
        {
            get { return _RmName; }
            set { _RmName = value; }
        }
        [DataMember]
        public string RamName
        {
            get { return _RamName; }
            set { _RamName = value; }
        }
        [DataMember]
        public string GmoGenAsstName
        {
            get { return _GmoGenAsstName; }
            set { _GmoGenAsstName = value; }
        }
        [DataMember]
        public string GmoGenName
        {
            get { return _GmoGenName; }
            set { _GmoGenName = value; }
        }
    }
    #endregion

    //FOR ITEM PRICING

    [DataContract]
    public class ListOfItemsResponse : Responses
    {
        [DataMember]
        public List<ListOfItems> ListOfItems { get; set; }
    }
    public class ListOfItems
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemPrice { get; set; }

    }

}