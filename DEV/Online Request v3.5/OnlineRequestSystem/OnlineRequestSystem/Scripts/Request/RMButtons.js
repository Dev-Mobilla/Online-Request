$("#Received").on('click', function (e) {
    bootbox.confirm({
        title: "Confirmation",
        message: "Are you sure you received the item/s from MMD ?",
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
                e.preventDefault();
                $.ajax({
                    type: "POST",
                    url: Url + '/SDC/Received',
                    data: {
                        ReqNo: ReqNO
                    },
                    success: function (result) {
                        if (result.status) {
                            bootbox.alert({
                                message: result.msg,
                                size: "small",
                                callback: function () {
                                    var dialog = bootbox.dialog({
                                        message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Redirecting..</p>',
                                        closeButton: false
                                    });
                                    window.location.href = Url + '/satellite-distribution-center';
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

$("#InTransitBranch").on('click', function (e) {
    bootbox.confirm({
        title: "Confirmation",
        message: "Are you sure you want to deliver the item/s to branch ?",
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
                e.preventDefault();
                $.ajax({
                    type: "POST",
                    url: Url + '/SDC/InTransitBranch',
                    data: {
                        ReqNo: ReqNO
                    },
                    success: function (result) {
                        if (result.status) {
                            bootbox.alert({
                                message: result.msg,
                                size: "small",
                                callback: function () {
                                    var dialog = bootbox.dialog({
                                        message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Redirecting..</p>',
                                        closeButton: false
                                    });
                                    window.location.href = Url + '/sdc?selected=RECEIVED';
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