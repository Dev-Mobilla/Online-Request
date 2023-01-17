using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using OnlineRequestWCF.Request.Models;
using log4net;
using System.Reflection;
using OnlineRequestWCF.Request.Methods;
using log4net.Config;


// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Request : IORequest 
{
    private readonly ILog _RequestLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
   
    public string GetData()
    {
        return string.Format("String Test!");
    }
    public LoginResponse Login(string username, string password)
    {
        var x = new Queries(_RequestLog);
        return x.Authentication(username, password);
    }

    public BranchNameResponse GetBranchName(string BranchCode, string Region, string ZoneCode)
    {
        var x = new Queries(_RequestLog);
        return x.GetBranchName(BranchCode, Region, ZoneCode);
    }

    public DivisionNameResponse GetDivisionName(string divcode)
    {
        var x = new Queries(_RequestLog);
        return x.GetDivisionName(divcode);
    }

    public DivisionNameResponse GetIRDivisionName(string costcenter) 
    {
        var x = new Queries(_RequestLog);
        return x.GetIRDivisionName(costcenter);
    }
    public BranchCodeResponses GetBranchCode(string branchName)
    {
        var x = new Queries(_RequestLog);
        return x.GetBranchCode(branchName);
    }

    public DivisionCostCenterResponses GetDivisionCostCenter(string divacro)
    {
        var x = new Queries(_RequestLog);
        return x.GetDivisionCostCenter(divacro);
    }

    public IRDivCodeResponses GetIRDivCode(string DivCode)
    {
        var x = new Queries(_RequestLog);
        return x.GetIRDivCode(DivCode);
    }

    public DivManResourceIDResponses GetDivisionApproverResourceID(string zonecode, string DivisionManager)
    {
        var x = new Queries(_RequestLog);
        return x.GetDivisionApproverResourceID(zonecode, DivisionManager);
    }

    public DivisionDetailsResponses GetDivisionDetails(string DivAcro)
    {
        var x = new Queries(_RequestLog);
        return x.GetDivisionDetails(DivAcro);
    }

    public DivisionAcroResponses GetDivisionAcro(string divName)
    {
        var x = new Queries(_RequestLog);
        return x.GetDivisionAcro(divName);
    }

    public ListOfBranchesResponse ListOfBranches(string zonecode)
    {
        var x = new Queries(_RequestLog);
        return x.GetListOfBranches(zonecode);
    }

    public ListOfDivisionResponse ListOfDivisions()
    {
        var x = new Queries(_RequestLog);
        return x.GetListOfDivisions();
    }

    public ListOfAreaResponse ListOfAreas(string zonecode)
    {
        var x = new Queries(_RequestLog);
        return x.GetListOfAreas(zonecode);
    }

    public ListOfRegionResponse ListOfRegions(string zonecode)
    {
        var x = new Queries(_RequestLog);
        return x.GetListOfRegions(zonecode);
    }
    public ListOfSDCResponse ListOfSDCs(string CostCenter)
    {
        var x = new Queries(_RequestLog);
        return x.GetListOfSDC(CostCenter);
    }

    public ListOfIRDivisionResponse ListIRDivisions(string zonecode) 
    {
        var x = new Queries(_RequestLog);
        return x.ListIRDivisions(zonecode);
    }

    #region CASH REQUEST
    public CashRequestApproversResponse ListOfCashRequestApprovers(string zonecode, string class_04, string region, string areacode, string task)
    {
        var x = new Queries(_RequestLog);
        return x.CashRequestApprovers(zonecode, class_04, region, areacode, task);
    }
    #endregion

}
