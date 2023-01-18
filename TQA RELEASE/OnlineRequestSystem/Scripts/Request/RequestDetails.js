function onCreateBegin() {
    $(".loginloader").modal('show');
}

$('#loginmodal').on('hidden.bs.modal', function () {
    window.location.reload(true);
});

function onCreateSuccess(result) {
    if (result.status) {
        bootbox.alert({
            message: result.msg,
            size: "small",
            callback: function () {
                var dialog = bootbox.dialog({
                    message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Please wait..</p>',
                    closeButton: false
                });
                if (!result.retURL) {
                    bootbox.alert("Error 404 . Undefined route : " + Url + '/' + result.retURL);
                } else {
                    window.location.href = Url + '/' + result.retURL;
                }
            }
        });
        $(".loginloader").modal('hide');
    } else {
        $(".loginloader").modal('hide');
        bootbox.alert(result.msg);
    }
}
function onCreateFail(jqXHR, exception) {
    var errmsg = '';
    if (jqXHR.status === 0) {
        errmsg = 'Not connected. \n Please verify your network connection.';
        bootbox.alert(errmsg);
    } else if (jqXHR.status === 404) {
        errmsg = 'Requested page not found. [404]';
        bootbox.alert(errmsg);
    } else if (jqXHR.status === 500) {
        errmsg = 'Internal Server Error [500].';
        bootbox.alert(errmsg);
    } else if (exception === 'parsererror') {
        errmsg = 'Requested JSON parse failed.';
        bootbox.alert(errmsg);
    } else if (exception === 'timeout') {
        errmsg = 'Time out error.';
        bootbox.alert(errmsg);
    } else if (exception === 'abort') {
        errmsg = 'Ajax request aborted.';
        bootbox.alert(errmsg);
    } else {
        errmsg = 'Uncaught Error.\n' + jqXHR.responseText;
        bootbox.alert(errmsg);
    }
}