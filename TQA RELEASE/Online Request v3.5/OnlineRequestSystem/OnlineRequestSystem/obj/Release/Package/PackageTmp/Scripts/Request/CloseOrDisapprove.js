function empty() {
    var Remarks = $('#ClosingRemarks').val();
    var ReceivedDate = $('#pickDate').val();
    var x;
    x = document.getElementById("ClosingRemarks").value;
    y = document.getElementById("pickDate").value;
    if (x == "") {
        bootbox.alert('Please provide Remarks.');
        return false;
    }
    else if (y == "") {
        bootbox.alert('Please put date.');
        return false;
    } else {
        $("#_CloseReq").modal('hide');
        $(".loginloader").modal('show');
        $.ajax({
            type: "POST",
            url: Url + '/CloseRequest/closeRequest',
            data: {
                ReqNo: ReqNO,
                Remarks: Remarks,
                ReceivedDate: ReceivedDate
            },
            success: function (result) {
                if (result.status == true) {
                    $(".loginloader").modal('hide');
                    $('#closeModalword').html(result.msg);
                    $('#closeModal').modal('show');
                    if ($("#divRequest").val() == 1) {
                        sumDiv();
                    }
                    else {
                        sumBranch();
                    }
                } else {
                    $(".loginloader").modal('hide');
                    $('#loginmodalword').html(result.msg);
                    $('#loginmodal').modal('show');
                }
            },
            error: function () {
                $('#loginmodalword').html('Error, Unable to Process Request.');
                $('#loginmodal').modal('show');
            },
        });
    };
};

$("#DisApproved").on('click', function (e) {
    var Remarks = $('#DisapproveRemarks').val();
    $(".loginloader").modal('show');
    $("#_DisApprove").modal('hide');
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: Url + '/DisapproveReq/DisapproveRequest',
        data: {
            ReqNo: ReqNO,
            Remarks: Remarks
        },
        success: function (result) {
            if (result.status == true) {
                $(".loginloader").modal('hide');
                $('#closeModalword').html(result.msg);
                $('#closeModal').modal('show');
            } else {
                $(".loginloader").modal('hide');
                $('#loginmodalword').html(result.msg);
                $('#loginmodal').modal('show');
            }
        },
        error: function () {
            $('#loginmodalword').html('Error, Unable to Process Request.');
            $('#loginmodal').modal('show');
        },
    });
});

function Div2CheckRemarks() {
    var x;
    x = document.getElementById("Div2Remarks").value;
    if (x == "") {
        $('#loginmodalword').html('Please provide Remarks.');
        $('#loginmodal').modal('show');
        return false;
    }
};

function Div3CheckRemarks() {
    var x;
    x = document.getElementById("Div2Remarks").value;
    if (x == "") {
        $('#loginmodalword').html('Please provide Remarks.');
        $('#loginmodal').modal('show');
        return false;
    }
};