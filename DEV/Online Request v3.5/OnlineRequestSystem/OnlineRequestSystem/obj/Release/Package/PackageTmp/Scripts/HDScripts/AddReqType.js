function onCreateBegin() {
    $(".loginloader").modal('show');
    $('#_AddRequestType').modal('hide');
    $('#_UpdateModal').modal('hide');
}

$('#loginmodal').on('hidden.bs.modal', function () {
    window.location.reload(true);
})

function onCreateSuccess(result) {
    if (result.status) {
        $(".loginloader").modal('hide');
        $('#loginmodalword').html(result.msg);
        $('#loginmodal').modal('show');
    } else {
        $(".loginloader").modal('hide');
        $('#loginmodalword').html(result.msg);
        $('#loginmodal').modal('show');
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

//For Update Request Type
$('.Update').click(function () {
    var update = $(this).attr('data-value');
    $('#IDforUpdate').val($('#u1_' + update).text());
    $('#RequestType').val($('#u2_' + update).text());
    ($('#u3_' + update).text() == "1") ? $('#isAMApproval').prop('checked', true) : $('#isAMApproval').prop('checked', false);
    ($('#u4_' + update).text() == "1") ? $('#isRMApproval').prop('checked', true) : $('#isRMApproval').prop('checked', false);
    ($('#u5_' + update).text() == "1") ? $('#isGMApproval').prop('checked', true) : $('#isGMApproval').prop('checked', false);

    // Division 1
    if ($('#u6_' + update).text() == "1") {
        $('#upCheckDiv1').prop('checked', true);
        $('#upDiv1').show()
        $('#upDPdiv1 option').map(function () {
            if ($(this).text() == $('#u10_' + update).text()) return this;
        }).attr('selected', 'selected');
    } else {
        $('#upCheckDiv1').prop('checked', false);
        $('#upDiv1').hide();
    }
    ($('#u7_' + update).text() == "1") ? $('#isPresidentApproval').prop('checked', true) : $('#isPresidentApproval').prop('checked', false);
    // Division 2
    if ($('#u8_' + update).text() == "1") {
        $('#upCheckDiv2').prop('checked', true)
        $('#upDiv2').show();
        $('#upDPdiv2 option').map(function () {
            if ($(this).text() == $('#u11_' + update).text()) return this;
        }).attr('selected', 'selected');
    } else {
        $('#upCheckDiv2').prop('checked', false)
        $('#upDiv2').hide();
    }
    // Division 3
    if ($('#u9_' + update).text() == "1") {
        $('#upCheckDiv3').prop('checked', true)
        $('#upDiv3').show();
        $('#upDPdiv3 option').map(function () {
            if ($(this).text() == $('#u12_' + update).text()) return this;
        }).attr('selected', 'selected');
    }
    else {
        $('#upCheckDiv3').prop('checked', false)
        $('#upDiv3').hide();
    }
});

$('#upCheckDiv1').click(function () {
    if (this.checked) {
        $('#upDiv1').show();
        $('#upDPdiv1').prop('disabled', false);
    } else {
        $('#upDiv1').hide();
        $('#upDPdiv1').prop('disabled', true).val('');
    }
});
$('#upCheckDiv2').click(function () {
    if (this.checked) {
        $('#upDiv2').show();
        $('#upDPdiv2').prop('disabled', false);
    } else {
        $('#upDiv2').hide();
        $('#upDPdiv2').prop('disabled', true).val('');
    }
});
$('#upCheckDiv3').click(function () {
    if (this.checked) {
        $('#upDiv3').show();
        $('#upDPdiv3').prop('disabled', false)
    } else {
        $('#upDiv3').hide();
        $('#upDPdiv3').prop('disabled', true).val('');
    }
});

// Add Request Type
$('#forDiv1').click(function () {
    if (this.checked) {
        $('#Div1').show();
        $('#DPdiv1').prop('disabled', false);
    } else {
        $('#Div1').hide();
        $('#DPdiv1').prop('disabled', true).val('');
    }
});
$('#forDiv2').click(function () {
    if (this.checked) {
        $('#Div2').show();
        $('#DPdiv2').prop('disabled', false);
    } else {
        $('#Div2').hide();
        $('#DPdiv2').prop('disabled', true).val('');
    }
});
$('#forDiv3').click(function () {
    if (this.checked) {
        $('#Div3').show();
        $('#DPdiv3').prop('disabled', false)
    } else {
        $('#Div3').hide();
        $('#DPdiv3').prop('disabled', true).val('');
    }
});

$(document).ready(function () {
    $('#Div1').hide();
    $('#Div2').hide();
    $('#Div3').hide();
    $('.AM').prop('checked', true);
    $('.RM').prop('checked', true);
    $('.GM').prop('checked', true);
});