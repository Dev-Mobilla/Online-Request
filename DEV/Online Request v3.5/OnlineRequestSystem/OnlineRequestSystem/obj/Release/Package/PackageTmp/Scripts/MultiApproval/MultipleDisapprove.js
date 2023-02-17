$('#btnMultiDisapprove').on('click', function () {
    if (checkBoxChecked() == false) {
        bootbox.alert({ message: 'Please check any request to process.', size: 'small' });
        return false;
    }
    bootbox.confirm({
        title: "Confirmation",
        message: "Are you sure you want to <b>DISAPPROVE</b> all selected requests?",
        buttons: {
            confirm: { label: '<span class="glyphicon glyphicon-ok-sign"></span> Yes ', className: 'btn-danger ddd' },
            cancel: { label: '<span class="glyphicon glyphicon-remove-sign"></span> No', className: 'btn-default pull-right' }
        }, callback: function (result) {
            if (result == true) {
                var dialog = bootbox.dialog({
                    message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Processing requests..</p>',
                    closeButton: false
                });
                var ReqNolist = [];
                var ForPOList = [];
                var approver = $('#approver').val();

                $(".chkBox:checkbox:checked").each(function () {
                    ReqNolist.push($(this).closest("tr").find('td:eq(3)').text());
                    ForPOList.push($(this).closest("tr").find('td:eq(12)').text());
                });

                $.ajax({
                    type: "POST",
                    url: Url + "/MultipleApproving/MultipleDisapprove",
                    data: { ReqNo: JSON.stringify(ReqNolist), ForPO: JSON.stringify(ForPOList), approver: approver },
                    success: function (result) {
                        if (result.status = true) {
                            dialog.modal('hide');
                            bootbox.alert(result.msg, function () {
                                var dialog = bootbox.dialog({
                                    message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Reloading page..</p>',
                                    closeButton: false
                                });
                                window.location.reload(true);
                            });
                        }
                    },
                    error: function () {
                        bootbox.alert('Error, Unable to process request');
                    }
                })
            }
        }
    });
});

function checkBoxChecked() {
    var checkboxes = false;
    $('.chkBox').each(function () {
        if ($(this).is(':checked')) {
            checkboxes = true;
        }
    });
    return checkboxes;
}