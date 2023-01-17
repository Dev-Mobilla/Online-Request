$(document).ready(function () {
    $('.itemDes, .UnitQty, .UnitCost, .selectRT').prop('required', 'required');
    $('#insert-more').click(function () {
        var Length = $('#myTable tr:last').attr('id').replace('TR', '');
        if (Length > 18) {
            bootbox.alert("You've reached the maximum limit of item requests.");
        } else {
            var xLength = parseInt(Length) + 1;
            var count = parseInt(Length) + 2;
            $('.count').text(count)
            $('#myTable tbody').append($('#myTable tbody tr:last').clone().attr('id', 'TR' + xLength));
            $('#myTable tbody #TR' + xLength + ' input').val('');
            $('#myTable tr').each(function () {
                $('#TR' + xLength + ' .itemCount').text(count)
                $('#TR' + xLength + ' .itemDes').removeAttr('name').attr('name', 'ReqItems[' + xLength + '].ItemDescription').removeAttr('id').attr('id', 'reqName' + xLength);
                $('#TR' + xLength + ' .UnitQty').removeAttr('name').attr('name', 'ReqItems[' + xLength + '].ItemQty').removeAttr('id').attr('id', 'reqQty' + xLength);
                $('#TR' + xLength + ' .itemUnit').removeAttr('name').attr('name', 'ReqItems[' + xLength + '].ItemUnit').removeAttr('id').attr('id', 'reqUnit' + xLength);
            });
        }
    });
});

function resetRowCounts() {
    xcount = $('#myTable tbody').children().length;

    $('#myTable tr').each(function (index) {
        $('#myTable tbody tr:eq(' + index + ')').attr('id', 'TR' + index);
        $('#TR' + index + ' .itemCount').text(index + 1)
        $('#TR' + index + ' .itemDes').attr('name', 'ReqItems[' + index + '].ItemDescription').attr('id', 'reqName' + index);
        $('#TR' + index + ' .UnitQty').attr('name', 'ReqItems[' + index + '].ItemQty').attr('id', 'reqQty' + index);
        $('#TR' + index + ' .itemUnit').attr('name', 'ReqItems[' + index + '].ItemUnit').attr('id', 'reqUnit' + index);
        index++;
    });

};

$('#myTable').on('click', '#removeRow', function () {
    rowCount = $('#myTable tbody').children().length;
    if (rowCount == 1) {
        return false;
    } else {
        $('#myTable tbody tr').removeAttr('id');
        $('.itemCount').text('');
        $('.itemDes').removeAttr('name').removeAttr('id');
        $('.UnitQty').removeAttr('name').removeAttr('id');
        $('.itemUnit').removeAttr('name').removeAttr('id');
        $(this).parent().closest("tr").remove();
    }
    resetRowCounts();
})

function itemNameChecking() {
    var $elems = $('.itemDes');
    var values = [];
    var isDuplicated = false;

    $elems.each(function () {
        if (!this.value.trim()) return true;

        if (values.indexOf(this.value.trim()) !== -1) {
            isDuplicated = true;
            return false;
        }

        values.push(this.value.trim());
    });

    return isDuplicated;
}


function onCreateBegin() {
    $(".loginloader").modal('show');
    var description = $("#requestDescription").val();
    var items = $('.itemDes').val();
    if (description.trim() === null || description.trim() === '') {
        $(".loginloader").modal('hide');
        bootbox.alert("Please put description!");
        return false;
    } else if (items.trim() === null || items.trim() === '') {
        $(".loginloader").modal('hide');
        bootbox.alert("Please put item name!");
        return false;
    } else if (itemNameChecking()) {
        $(".loginloader").modal('hide');
        bootbox.alert("Duplicate item names!");
        return false;
    }
}

$('#loginmodal').on('hidden.bs.modal', function () {
    window.location.reload(true);
})

function onCreateSuccess(result) {
    if (result.status == true) {
        $(".loginloader").modal('hide');
        $('#SubmitButton').prop('disabled', true);
        bootbox.alert(result.msg, function () {
            var dialog = bootbox.dialog({
                message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Redirecting..</p>',
                closeButton: false
            });
            window.location.href = Url + '/open-requests';
        });
    }
    else {
        if (result.resCode = "001") {
            $(".loginloader").modal('hide');
            bootbox.alert({ title: 'Unable to process request.', message: result.msg });
        } else {
            $(".loginloader").modal('hide');
            bootbox.alert(result.msg);
        }
    }
}
function onCreateFail(jqXHR, exception) {
    var errmsg = '';
    if (jqXHR.status === 0) {
        errmsg = 'Not connected. \n Please verify your network connection.';
        bootbox.alert(errmsg);
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

$("#attDiagnostic").on('click', function () {
    alert("sadfasd");
});