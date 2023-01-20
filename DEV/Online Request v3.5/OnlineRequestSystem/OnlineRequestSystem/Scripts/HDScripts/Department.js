$('.Update').click(function () {
    var update = $(this).attr('data-value');
    $('#uptDeptName').val($('#u1_' + update).text());
    $('#updtDeptManager').val($('#u2_' + update).text());
    $('#upt_ResID').val($('#u3_' + update).text());
    $('#uptDivision').val($('#u4_' + update).text());
    $('#uptdeptID').val($('#u6_' + update).text());
});

$('#loginmodal').on('hidden.bs.modal', function () {
    window.location.reload(true);
});

function onCreateBegin() {
    $(".loginloader").modal('show');
    $("#_UpdateModal").modal('hide');
    $("#_AddDepartment").modal('hide');
}

function onCreateSuccess(result) {
    if (result.status == true) {
        $(".loginloader").modal('hide');
        bootbox.alert(result.msg, function () { window.location.reload(true); });
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
        bootbox.alert(errmsg);
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

$('#uptDivision').on('change', function (ix, val) {
    var divName = $('#uptDivision :selected').val();
    if (divName == "") {
        $('#dept_divcode').val('');
        $('#Employees').empty();
        return false;
    }
    $.ajax({
        type: "POST",
        url: Url + '/MMD/DivisionDetails',
        data: { divName: divName },
        success: function (result) {
            if (result.status == true) {
                var dialog = bootbox.dialog({
                    message: '<p class="text-center">Loading data...</p>',
                    closeButton: false
                });

                var select = $('#uptEmployees');
                select.empty();

                for (var i = 0; i < result.employee.length; i++) {
                    select.append('<option value="' + result.employee[i].Text + '">' + result.employee[i].Text + '</option>');
                }
                $('#uptDivcode').val(result.divacroo);
                dialog.modal('hide');
            } else {
                bootbox.alert('Unable to Process Request. Pls call Dev');
            }
        },
        error: function () {
            bootbox.alert('Unable to Process Request. Pls call Dev');
        },
    });
});

//function DisplayResourceID(name) {
//    $.ajax({
//        type: "POST",
//        url: Url + '/MMD/GetResourceID',
//        data: { fullname: name },
//        success: function (result) {
//            if (result.status == true) {
//                $('#upt_ResID').val(result.data);

//            } else {
//                bootbox.alert(result.msg);
//            }
//        },
//        error: function () {
//            bootbox.alert('Unable to Process Request. Pls call Dev');
//        },
//    });
//}

$('#updateDeptButton').on('click', function () {
    if ($('#uptDeptName').val().trim() == "" || $('#uptDeptName').val().trim() == null) {
        bootbox.alert('Please input Department name');
        return false;
    }
    if ($('#updtDeptManager').val().trim() == "" || $('#updtDeptManager').val().trim() == null) {
        bootbox.alert('Please input Department Manager');
        return false;
    }
});

$('#uptEmployees').on('change', function (ix, val) {
    var employee = $('#uptEmployees :selected').val();
    $.ajax({
        type: "POST",
        url: Url + '/MMD/GetResourceID',
        data: { fullname: employee },
        success: function (result) {
            if (result.status == true) {
                $('#upt_ResID').val(result.data);
                dialog.modal('hide');
            } else {
                bootbox.alert(result.msg);
            }
        },
        error: function () {
            bootbox.alert('Unable to Process Request. Pls call Dev');
        },
    });
});