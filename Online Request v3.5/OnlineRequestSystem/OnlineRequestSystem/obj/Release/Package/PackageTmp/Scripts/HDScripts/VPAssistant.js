$('.Update').click(function () {
    var update = $(this).attr('data-value');
    $('#assistantName').val($('#u1_' + update).text());
    $('#Division').val($('#u2_' + update).text());
    $('#Zonecode').val($('#u3_' + update).text());
    $('#UserID').val($('#u4_' + update).text());
    $('#vp_id').val($('#u5_' + update).text());
});

$('#loginmodal').on('hidden.bs.modal', function () {
    window.location.reload(true);
});

function onCreateBegin() {
    $(".loginloader").modal('show');
    $("#_UpdateModal").modal('hide');
    $("#_AddDivision").modal('hide');
}

function onCreateSuccess(result) {
    if (result.status == true) {
        $(".loginloader").modal('hide');
        window.location.reload(true);
        bootbox.alert(result.msg);
    } else {
        $(".loginloader").modal('hide');
        bootbox.alert(result.msg);
        $('#form0')[0].reset();
    }
}

function onCreateFail(jqXHR, exception) {
    var errmsg = '';
    if (jqXHR.status === 0) {
        errmsg = 'Not connected. \n Please verify your network connection.';
        alert(errmsg);
    } else if (jqXHR.status == 404) {
        errmsg = 'Requested page not found. [404]';
        alert(errmsg);
    } else if (jqXHR.status == 500) {
        errmsg = 'Internal Server Error [500].';
        alert(errmsg);
    } else if (exception === 'parsererror') {
        errmsg = 'Requested JSON parse failed.';
        alert(errmsg);
    } else if (exception === 'timeout') {
        errmsg = 'Time out error.';
        alert(errmsg);
    } else if (exception === 'abort') {
        errmsg = 'Ajax request aborted.';
        alert(errmsg);
    } else {
        errmsg = 'Uncaught Error.\n' + jqXHR.responseText;
        alert(errmsg);
    }
}