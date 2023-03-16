using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using OnlineRequestWCF.Request.Models;


// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
[ServiceContract]
public interface IORequest
{
    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/GetData")]
    string GetData();

    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/Login")]
    LoginResponse Login(string username, string password);

    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/GetBranchName/?BranchCode={BranchCode}&Region={Region}&ZoneCode={ZoneCode}")]
    BranchNameResponse GetBranchName(string BranchCode, string Region, string ZoneCode);

    [OperationContract]
    [WebInvoke(Method = "GET",
      RequestFormat = WebMessageFormat.Json,
      ResponseFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest,
      UriTemplate = "/GetDivisionName/?divcode={divcode}")]
    DivisionNameResponse GetDivisionName(string divcode);

    [OperationContract]
    [WebInvoke(Method = "GET",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          UriTemplate = "/GetIRDivisionName/?costcenter={costcenter}")]
    DivisionNameResponse GetIRDivisionName(string costcenter);

    [OperationContract]
    [WebInvoke(Method = "GET",
      RequestFormat = WebMessageFormat.Json,
      ResponseFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest,
      UriTemplate = "/GetBranchCode/?branchName={branchName}")]
    BranchCodeResponses GetBranchCode(string branchName);

    [OperationContract]
    [WebInvoke(Method = "GET",
      RequestFormat = WebMessageFormat.Json,
      ResponseFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest,
      UriTemplate = "/GetDivisionCostCenter/?divacro={divacro}")]
    DivisionCostCenterResponses GetDivisionCostCenter(string divacro);

    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetIRDivCode/?DivCode={DivCode}")]
    IRDivCodeResponses GetIRDivCode(string DivCode);

    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetDivisionApproverResourceID/?zonecode={zonecode}&DivisionManager={DivisionManager}")]
    DivManResourceIDResponses GetDivisionApproverResourceID(string zonecode, string DivisionManager);

    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetDivisionDetails/?DivAcro={DivAcro}")]
    DivisionDetailsResponses GetDivisionDetails(string DivAcro);

    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetDivisionAcro/?divName={divName}")]
    DivisionAcroResponses GetDivisionAcro(string divName);

    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetListOfBranches/?zonecode={zonecode}")]
    ListOfBranchesResponse ListOfBranches(string zonecode);

    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetListOfDivisions")]
    ListOfDivisionResponse ListOfDivisions();

    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetListOfAreas/?zonecode={zonecode}")]
    ListOfAreaResponse ListOfAreas(string zonecode);

    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetListOfRegions/?zonecode={zonecode}")]
    ListOfRegionResponse ListOfRegions(string zonecode);

    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetListOfSDC/?CostCenter={CostCenter}")]
    ListOfSDCResponse ListOfSDCs(string CostCenter);


    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetIRDivisions/?zonecode={zonecode}")]
    ListOfIRDivisionResponse ListIRDivisions(string zonecode);



    #region CASH REQUEST
    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/GetCashRequestApprovers/?zonecode={zonecode}&class_04={class_04}&region={region}&areacode={areacode}&job_title={job_title}")]
    CashRequestApproversResponse ListOfCashRequestApprovers(string zonecode, string class_04, string region, string areacode, string job_title);
    #endregion


    //For Item Pricing
    [OperationContract]
    [WebInvoke(Method = "GET",
     RequestFormat = WebMessageFormat.Json,
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest,
     UriTemplate = "/GetAllItems")]
    ListOfItemsResponse ListOfItems();

    [OperationContract]
    [WebInvoke(Method = "GET",
    RequestFormat = WebMessageFormat.Json,
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    UriTemplate = "/SearchItem/?itemDetails={searchCriteria}")]
    ListOfItemsResponse SearchItem(string searchCriteria);
}


