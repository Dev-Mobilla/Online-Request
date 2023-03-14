$(document).ready(function () {

    var isProcessed = $('#isProcessed').val();
    var MyReqMMD = (typeof $('#isMMDuser').val() == 'undefined') ? 0 : $('#isMMDuser').val();

    if (isProcessed == 1 || MyReqMMD != 1) {
        $('.StockCheckbox').each(function () {
            $(this).attr('disabled', 'disabled');
        });

        if (typeof $('#split-Req') != 'undefined') {
            /*document.getElementById("split-Req").style.display = "none"*/
            $('#split-Req').hide();
        }

        $('input[name="pricePerItem"]').each(function () {
            if (document.getElementById(this.id).disabled != true) {
                document.getElementById(this.id).disabled = true;
            }
        });
        $('.itemPriceBtn').each(function () {
            if (document.getElementById(this.id).disabled != true) {
                document.getElementById(this.id).disabled = true;
            }
        });

    }
    
});

$('#split-Req').on('click', function () {

    var inputsDesc = [];
    var inputsPrice = [];
    var outOfStockDesc = [];
    var inStockDesc = [];

    $('.itemDesc').each(function () {
        if ($(this).text().trim() != "") {
            inputsDesc.push($(this).text().toString().trim());
        }
    });

    $('input[name="pricePerItem"]').each(function () {
        if ($(this).val().trim() != "") {
            inputsPrice.push($(this).val().toString().trim());
        }
    });

    $(".StockCheckbox:checkbox:checked").each(function () {
        inStockDesc.push($(this).closest("tr").find('td:eq(1)').text());
    });

    $('.StockCheckbox').each(function () {
        if (!$(this).is(':checked')) {
            outOfStockDesc.push($(this).closest("tr").find('td:eq(1)').text());
        }
    });

    if (inputsPrice.length == outOfStockDesc.length) {
        if (inStockDesc.length != inputsDesc.length && outOfStockDesc.length != inputsDesc.length) {
            bootbox.confirm({
                title: "Confirmation",
                message: "Are you sure you want to split this requests?",
                size: 'small',
                buttons: {
                    confirm: {
                        label: '<span cla="glyphicon glyphicon-ok-sign"></span> Yes ',
                        className: 'btn-danger ddd'
                    },
                    cancel: {
                        label: '<span class=lyphicon glyphicon-remove-sign"></span> No',
                        className: 'btn-default pull-right'
                    }
                }, callback: function (result) {
                    if (result == true) {
                        var dialog = bootbox.dialog({
                            message: '<p class="text-cente><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Processing requests..</p>',
                            closeButton: false
                        });

                        $.ajax({
                            type: "POST",
                            url: Url + "/MMD/SplitRequest",
                            traditional: true,
                            data: { ReqNo: ReqNO, 'PricePerItem': inputsPrice, InStock: JSON.stringify(inStockDesc), OutOfStock: JSON.stringify(outOfStockDesc) },
                            success: function (result) {
                                if (result.status == true) {

                                    dialog.modal('hide');

                                    var newUrl = insertParam("ReqNo", ReqNO + "-A");

                                    window.open("request-details?" + newUrl);

                                    var msg = "Successfully Split Request! Please see split request in the new tab.";

                                    bootbox.alert({
                                        title: "System Message",
                                        message: msg,
                                        size: "small",
                                        callback: function () {
                                            var dialog = bootbox.dialog({
                                                message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Redirecting..</p>',
                                                closeButton: false
                                            });

                                            location.reload();
                                        }
                                    });
                                }
                                else {
                                    dialog.modal('hide');
                                    var msg = "Unable to process request.";
                                    bootbox.alert({
                                        title: "Error",
                                        message: msg,
                                        size: "small"
                                    });
                                }
                            },
                            error: function () {
                                dialog.modal('hide');
                                var msg = 'Unable to process request';
                                bootbox.alert({
                                    title: "Error",
                                    message: msg,
                                    size: "small"
                                });
                            }
                        });
                    }
                }
            });
        }
        else {
            var msg = "No need to split request.";
            bootbox.alert({
                title: "System Message",
                message: msg,
                size: "small"
            });
            return;
        }
    }
    else {
        var msg = "Please review and provide price for out of stock item/s.";
        bootbox.alert({
            title: "System Message",
            message: msg,
            size: "small"
        });

        return;
    }

    function insertParam(key, value) {
        key = encodeURIComponent(key);
        value = encodeURIComponent(value);

        var kvp = document.location.search.substr(1).split('&');
        let i = 0;

        for (; i < kvp.length; i++) {
            if (kvp[i].startsWith(key + '=')) {
                let pair = kvp[i].split('=');
                pair[1] = value;
                kvp[i] = pair.join('=');
                break;
            }
        }

        if (i >= kvp.length) {
            kvp[kvp.length] = [key, value].join('=');
        }

        let params = kvp.join('&');

        return params;
    }
});

function checkBoxChecked() {
    var checkboxes = false;
    $('.StockCheckbox').each(function () {
        if ($(this).is(':checked')) {
            checkboxes = true;
        }
    });
    return checkboxes;
}

$(document).on('click', '#chkStock', function () {
    var Description = $(this).closest('tr').find('td:eq(1)').text();
    var Desc = Description.trim();
    if ($(this).prop('checked') == true) {

        $(this).closest("tr").find('.itemPriceBtn').prop('disabled', true);
        $(this).closest("tr").find('.priceTotal').val("");
        $(this).closest("tr").find('.priceTotal').prop('disabled', true);

        var inputs = $('input[name="pricePerItem"]')
        var finalPrice
        var overallTotalPrice = 0;
        inputs.each(function () {
            if ($(this).val() != "" || $(this).val() != 'undefined' || $(this).val() != 'NaN') {
                overallTotalPrice += parseFloat(document.getElementById(this.id).value.replace(/,/g, "")) || 0;
                finalPrice = overallTotalPrice.toFixed(2);
            }
        })

        finalPrice = finalPrice == "0.00" ? "" : finalPrice;

        document.getElementById("overallPrice").value = finalPrice.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        document.getElementById("_overallPrice").value = finalPrice;

        $.notify(Desc + " in-stock.", {
            position: "bottom right",
            className: "info"
        });

        //$.ajax({
        //    type: "POST",
        //    url: Url + '/MMD/UpdateStatusOfStocks',
        //    data: { ReqNo: ReqNO, description: Desc, status: "1" },
        //    success: function (result) {
        //        if (result.status) {
        //            $.notify(Desc + " in-stock.", {
        //                position: "bottom right",
        //                className: "info"
        //            });
        //        } else {
        //            $.notify(result.msg, {
        //                position: "bottom right",
        //                className: "error"
        //            });
        //        }
        //    },
        //    error: function () {
        //        $.notify("Unable to process request.", {
        //            position: "bottom right",
        //            className: "error"
        //        });
        //    },
        //});

    } else {

        $(this).closest("tr").find('.itemPriceBtn').removeAttr('disabled');
        $(this).closest("tr").find('.priceTotal').removeAttr('disabled');

        $.notify(Desc + " out of stock.", {
            position: "bottom right",
            className: "info"
        });

        //$.ajax({
        //    type: "POST",
        //    url: Url + '/MMD/UpdateStatusOfStocks',
        //    data: { ReqNo: ReqNO, description: Desc, status: "0" },
        //    success: function (result) {
        //        if (result.status) {
        //            $.notify(Desc + " out of stock.", {
        //                position: "bottom right",
        //                className: "info"
        //            });
        //        } else {
        //            $.notify(result.msg, {
        //                position: "bottom right",
        //                className: "error"
        //            });
        //        }
        //    },
        //    error: function () {
        //        $.notify("Unable to process request.", {
        //            position: "bottom right",
        //            className: "error"
        //        });
        //    },
        //});
    }
});