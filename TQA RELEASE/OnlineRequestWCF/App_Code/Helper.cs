using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Helper
/// </summary>
public class Helper
{
    public string ZoneCodeStringSelector(string zonecode)
    {
        string qString = "";
        if ((new[] { "VISAYAS", "VISMIN", "MINDANAO" }).Contains(zonecode.Trim()))
        {
            qString = "(zonecode = 'VISAYAS' OR zonecode = 'VISMIN' OR zonecode = 'MINDANAO') or (class_02 = 'MINDANAO' or class_02 = 'VISAYAS') ";
        }
        else
        {
            qString = "(zonecode = 'LUZON' OR zonecode = 'NCR' OR zonecode = 'LNCR') or (class_02 = 'LUZON' or class_02 = 'NCR')";
        }
        return qString;
    }

    public string DivisionZonecode(string zonecode)
    {
        string qString = "";
        if ((new[] { "VISAYAS", "VISMIN", "MINDANAO" }).Contains(zonecode))
        {
            qString = "VISMIN";
        }
        else
        {
            qString = "LUZON";
        }
        return qString;
    }

    public string AreaStringSelect(string zonecode)
    {
        string qString = "";
        if ((new[] { "VISAYAS", "VISMIN", "MINDANAO" }).Contains(zonecode))
        {
            qString = "(zonecode = 'VISAYAS' or zonecode = 'MINDANAO')";
        }
        else
        {
            qString = "(zonecode = 'LUZON' or zonecode = 'NCR')";
        }
        return qString;
    }
}