function onCreateBegin() {
    $(".loginloader").modal('show')
}

function onCreateSuccess(result) {
    if (result.status) {
        window.location = result.urlAction;
    } else {
        $(".loginloader").modal('hide');
        bootbox.alert({
            closeButton: false,
            message: result.msg
        });
    }
}

function onCreateFail(jqXHR, exception) {
    var errmsg = '';
    if (jqXHR.status === 0) {
        errmsg = 'Not connected. \n Please verify your network connection.';
        bootbox.alert(errmsg);
        $(".loginloader").modal('hide');
    } else if (jqXHR.status == 404) {
        errmsg = 'Requested page not found. [404]';
        bootbox.alert(errmsg);
    } else if (jqXHR.status == 500) {
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