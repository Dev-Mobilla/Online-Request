$(document).on('blur', '#txtMMD', function () {
    var check = $(this).prev().prop('checked'); 
    if (check == false) {
        $(this).val('0');
        $.notify("Checkbox required.", {
            position: "bottom right",
            className: "error"
        });
    } else {
        if ($(this).val() === '' || $(this).val() === null) {
            $(this).val('0');
        }
        var RequestNo = ReqNO;
        var Description = $(this).closest('tr').find('td:eq(1)').text();
        var Desc = Description.trim();
        var Quantity = $(this).val();

        $.ajax({
            type: "POST",
            url: Url + '/RequestItems/insertMMDItemStatus',
            data: { ReqNo: ReqNO, description: Desc, quantity: Quantity, isCheckedMMD: 1 },
            success: function (result) {
                if (result.status) {
                    $.notify(Desc + " : " + Quantity, {
                        position: "bottom right",
                        className: "success"
                    });
                } else {
                    $.notify(result.msg, {
                        position: "bottom right",
                        className: "error"
                    });
                }
            },
            error: function () {
                $.notify("Unable to process request.", {
                    position: "bottom right",
                    className: "error"
                });
            },
        });
    }
});

$(document).on('click', '#chkMMD', function () {
    var Description = $(this).closest('tr').find('td:eq(1)').text();
    var Desc = Description.trim();
    if ($(this).prop('checked') == true) {
        $.notify(Desc + " checked.", {
            position: "bottom right",
            className: "success"
        });
    } else {
        $(this).next().val('0');
        $.ajax({
            type: "POST",
            url: Url + '/RequestItems/insertMMDItemStatus',
            data: { ReqNo: ReqNO, description: Desc, quantity: 0, isCheckedMMD: 0 },
            success: function (result) {
                if (result.status) {
                    $.notify(Desc + " unchecked.", {
                        position: "bottom right",
                        className: "info"
                    });
                } else {
                    $.notify(result.msg, {
                        position: "bottom right",
                        className: "error"
                    });
                }
            },
            error: function () {
                $.notify("Unable to process request.", {
                    position: "bottom right",
                    className: "error"
                });
            },
        });
    }
});