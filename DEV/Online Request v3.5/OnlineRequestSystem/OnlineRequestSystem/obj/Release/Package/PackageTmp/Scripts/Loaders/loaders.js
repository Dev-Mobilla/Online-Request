function loadClick(x) {
    var message = "";
    switch (x) {
        case "Open":
            message = "Please wait while we load all open requests.";
            break;
        case "Pending":
            message = "Please wait while we load all pending requests.";
            break;
        case "Close":
            message = "Please wait while we load all closed requests.";
            break;
        case "Disapprove":
            message = "Please wait while we load all disapproved requests.";
            break;
        case "Division":
            message = "Please wait while we load all division approvers.";
            break;
        case "Department":
            message = "Please wait while we load all department approvers.";
            break;
        case "VP":
            message = "Please wait while we load VP assistant approvers.";
            break;
        case "SDC":
            message = "Please wait while we load all Satellite Distribution Center based on your zone.";
            break;
        case "Branch":
            message = "Please wait while we load branch requests.";
            break;
        case "Normal":
            message = "Please wait.";
            break;
        default:
            message = "Please wait.";
            break;
    }
    var dialog = bootbox.dialog({
        message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp; ' + message + '</p>',
        closeButton: false
    });
}

$('.mmdLoad').on('click', function () {
    var message = "Please wait";
    var dialog = bootbox.dialog({
        message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp; ' + message + '</p>',
        closeButton: false
    });
})