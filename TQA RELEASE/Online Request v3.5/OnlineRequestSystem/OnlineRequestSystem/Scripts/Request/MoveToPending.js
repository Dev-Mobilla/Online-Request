$('#movetopending').on('click', function () {
    bootbox.confirm({
        title: "Confirmation",
        message: "Are you sure you want to move this request to <b>Pending</b>?",
        buttons: {
            confirm: {
                label: '<span class="glyphicon glyphicon-ok-sign"></span> Confirm ',
                className: 'btn-warning ddd'
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
                $.ajax({
                    type: "POST",
                    url: Url + '/OpenRequest/MovetoPending',
                    data: {
                        RequestNo: ReqNO
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
                                    window.location.href = Url + '/pending-requests';
                                }
                            });
                        } else {
                            dialog.modal('hide');
                            bootbox.alert(result.msg);
                        }
                    },
                    error: function () {
                        dialog.modal('hide');
                        bootbox.alert('Error, Unable to Process Request.');
                    },
                });
            }
        }
    });
});