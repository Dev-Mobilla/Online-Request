//var ReqNO = $('#Req').val();
var isDivRequest = $('#divRequest').val();
$(document).ready(function () {
    $("#pickDate").datepicker({ maxDate: new Date() });

    $(".ReqDetailsBlock:selected").keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

    $("#printDiagnostic").on('click', function () {
        alert(ReqNO);
        window.location.href = Url + "/File/FileDownload?ReqNo=" + ReqNO;
    });

    //$("#ReprintSlip").on('click', function () {
    //    var RegionName = $("#RegionNamee").text();
    //    var isDivRequest = $("#divRequest").val();
    //    var requestorr = $("#requestorr").val();
    //    window.location.href = Url + "/MMD/GenerateIssuanceSlip?ReqNo=" + ReqNO + "&RegionName=" + RegionName + "&isDivRequest=" + isDivRequest + "&Requestor=" + requestorr;
    //});

    $("#Print").on('click', function () {
        var RegionName = $("#RegionNamee").text();
        var isDivRequest = $("#divRequest").val();
        var requestorr = $("#requestorr").val();
        var ReqTypeName = $("#RequestTypeName").text();
        var Description = $("#Description").text();
        var Office = $("#officeName").text();
        window.location.href = Url + "/OpenRequest/RequestDetailsPDF?ReqNo=" + ReqNO + "&RegionName=" + RegionName + "&isDivRequest=" + isDivRequest + "&Requestor=" + requestorr + "&ReqTypeName=" + encodeURIComponent(ReqTypeName) + "&Description=" + encodeURIComponent(Description) + "&Office=" + encodeURIComponent(Office);
    });
});