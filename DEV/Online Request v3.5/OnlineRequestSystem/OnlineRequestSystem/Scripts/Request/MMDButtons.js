$("#ProcessedPO").on('click', function (e) {

    var outOfStock = 0;
    var inStock = 0;

    var OverallTotal = $('#overallPrice').val();
    var inputsPrice = [];
    var inputsDesc = [];

    $('.priceTotal').each(function () {
        var val = $(this).val().replace(/,/g, '');
        if (val != "") {
            inputsPrice.push(val).toString();
        }
    });

    $('.itemDesc').each(function () {
        if ($(this).text().trim() != "") {
            inputsDesc.push($(this).text().toString());
        }
    });

    $(".StockCheckbox:checkbox:checked").each(function () {
        inStock = inStock + 1;
    });

    $('.StockCheckbox').each(function () {
        if (!$(this).is(':checked')) {
            outOfStock = outOfStock + 1;
        }
    });

    if (outOfStock == inputsDesc.length) {

        if (inputsPrice.length == inputsDesc.length) {
            ProcessPO("0");
        }
        else {
            var msg = "Please review and provide price for out of stock item/s.";
            bootbox.alert(msg);
            return;
        }
    }
    else if (inStock == inputsDesc.length) {
        ProcessPO("1");
    }
    else {
        console.log(outOfStock);
        console.log(inputsDesc.length)
        bootbox.alert("Please split items that are out of stocks.");
        return;
    }

    function ProcessPO(allStockStat) {
        bootbox.confirm({
            title: "Confirmation",
            message: "Are you sure you want to process PO?",
            //size: "small",
            buttons: {
                confirm: {
                    label: '<span class="glyphicon glyphicon-ok-sign"></span> Confirm ',
                    className: 'btn-danger ddd'
                },
                cancel: {
                    label: '<span class="glyphicon glyphicon-remove-sign"></span> Cancel',
                    className: 'btn-default pull-right'
                }
            },
            callback: function (result) {
                if (result == true) {
                    var dialog = bootbox.dialog({
                        message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Please wait..</p>',
                        closeButton: false
                    });
                    e.preventDefault();
                    $.ajax({
                        type: "POST",
                        url: Url + '/MMD/ProcessedPO',
                        traditional: true,
                        data: { 'ReqNo': ReqNO, 'OverallTotal': OverallTotal, 'TotalP': inputsPrice, 'desc': inputsDesc, 'allStockStat': allStockStat  },
                        success: function (result) {
                            if (result.status) {
                                dialog.modal('hide');
                                bootbox.alert({
                                    message: result.msg,
                                    size: "small",
                                    callback: function () {
                                        var dialog = bootbox.dialog({
                                            message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Redirecting..</p>',
                                            closeButton: false
                                        });
                                        //window.location.href = Url + '/MMDStatus?selected=' + encodeURIComponent("PROCESSED PO");
                                        window.location.href = Url + '/open-requests';
                                    }
                                });
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
                }
            }
        });
    }

});

$("#ForDelivery").on('click', function (e) {
    bootbox.confirm({
        title: "Confirmation",
        message: "Are you sure you want to deliver the item/s ?",
        buttons: {
            confirm: {
                label: '<span class="glyphicon glyphicon-ok-sign"></span> Confirm ',
                className: 'btn-danger ddd'
            },
            cancel: {
                label: '<span class="glyphicon glyphicon-remove-sign"></span> Cancel',
                className: 'btn-default pull-right'
            }
        },
        callback: function (result) {
            if (result == true) {
                var dialog = bootbox.dialog({
                    message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Please wait..</p>',
                    closeButton: false
                });
                e.preventDefault();
                var RegionName = $("#RegionNamee").text();
                $.ajax({
                    type: "POST",
                    url: Url + '/MMD/ForDelivery',
                    data: {
                        ReqNo: ReqNO,
                        RegionName: RegionName
                    },
                    success: function (result) {
                        if (result.status) {
                            var isDivRequest = $("#divRequest").val();
                            var requestorr = $("#requestorr").val();
                            dialog.modal('hide');
                            bootbox.alert({
                                message: result.msg, size: "small",
                                callback: function () {
                                    var dialog = bootbox.dialog({
                                        message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Redirecting..</p>',
                                        closeButton: false
                                    });
                                    window.setTimeout(function () {
                                        window.location.href = Url + '/MMD/viewMMDStatus?selected=' + encodeURIComponent("RECEIVED FROM SUPPLIER") + '&office=' + $('#office').val();
                                    }, 5000);
                                }
                            });
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
            }
        }
    });
});

$("#InTransit").on('click', function (e) {

    var hasCheck = [];
    var hasPrice = [];
    var hasStatus = [];

    $('.forMMD').each(function () {
        var isChecked = $(this).find('input[type=checkbox]').is(':checked');
            hasCheck.push(isChecked)
    })

    $('.mmdPrice').each(function () {
        var val = $(this).val()
        if (val != 0) {
            hasPrice.push(val).toString();
        }
    });

    $('.forMMD').each(function () {
        var selectedValue = $(this).find('i').attr('class');
        if (selectedValue.includes("txtSuccess") || selectedValue.includes("txtWarning") || selectedValue.includes("txtDanger")) {
            hasStatus.push(selectedValue)
        }

    });

    if ((hasCheck.length === hasPrice.length) && (hasPrice.length === hasStatus.length)) {
        bootbox.confirm({
            title: "Confirmation",
            message: "Are you sure you received the item/s?",
            //size: "small",
            buttons: {
                confirm: {
                    label: '<span class="glyphicon glyphicon-ok-sign"></span> Confirm ',
                    className: 'btn-danger ddd'
                },
                cancel: {
                    label: '<span class="glyphicon glyphicon-remove-sign"></span> Cancel',
                    className: 'btn-default pull-right'
                }
            },
            callback: function (result) {
                if (result == true) {
                    var dialog = bootbox.dialog({
                        message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Please wait..</p>',
                        closeButton: false
                    });
                    e.preventDefault();
                    $.ajax({
                        type: "POST",
                        url: Url + '/MMD/InTransit',
                        data: {
                            ReqNo: ReqNO
                        },
                        success: function (result) {
                            if (result.status) {
                                dialog.modal('hide');
                                bootbox.alert({
                                    message: result.msg,
                                    size: "small",
                                    callback: function () {
                                        var dialog = bootbox.dialog({
                                            message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Redirecting..</p>',
                                            closeButton: false
                                        });
                                        window.location.href = Url + '/MMD/viewMMDStatus?selected=' + encodeURIComponent("PROCESSED PO") + '&office=' + $('#office').val();
                                        //window.location.href = Url + '/MMDStatus?selected=' + encodeURIComponent("RECEIVED FROM SUPPLIER");
                                    }
                                });
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
                }
            }
        });
    }
    else {
        bootbox.dialog({
            message: '<p class="text-center">Please input quantity and update the status.</p>',
            closeButton: true
        });
        return;
    }
});