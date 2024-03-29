﻿$(document).on('blur', '#txtSDC', function () {
    var check = $(this).prev().prop('checked');
    var lastChild = $(this).closest("tr").find('td:last-child').text().trim();

    if (lastChild == 'Cancelled') {
        $.notify("Cannot be inputted if item is cancelled.", {
            position: "bottom right",
            className: "error"
        });
        $(this).val('0');
        return false;
    }
    else if (check == false) {
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
            url: Url + '/RequestItems/insertSDCItemStatus',
            data: { ReqNo: ReqNO, description: Desc, quantity: Quantity, isCheckedSDC: 1 },
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

$(document).on('click', '#chkSDC', function () {
    var Description = $(this).closest('tr').find('td:eq(1)').text();
    var Desc = Description.trim();
    var lastChild = $(this).closest("tr").find('td:last-child').text().trim();

    if (lastChild == 'Cancelled') {
        $.notify("Cannot be checked if item is cancelled.", {
            position: "bottom right",
            className: "error"
        });
        return false;
    }
    else if ($(this).prop('checked') == true) {
        $.notify(Desc + " checked.", {
            position: "bottom right",
            className: "success"
        });
    } else {
        $(this).next().val('0');
        $.ajax({
            type: "POST",
            url: Url + '/RequestItems/insertSDCItemStatus',
            data: { ReqNo: ReqNO, description: Desc, quantity: 0, isCheckedSDC: 0 },
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