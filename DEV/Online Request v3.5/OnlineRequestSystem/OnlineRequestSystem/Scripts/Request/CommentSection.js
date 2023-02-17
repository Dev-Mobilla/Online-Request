$('#comm_btn').click(function () {
    if ($('#commTextArea').val().trim() != "") {
        postComment();
    }
});

$('#_ShowComments').on('shown.bs.modal', function () {

    var messageBody = document.querySelector('#comment_body');
    messageBody.scrollTop = messageBody.scrollHeight - messageBody.clientHeight;

});

function postComment() {

    var reqNo = $('#Req').val();
    var comm = $('#commTextArea').val();

    $.ajax({
        url: Url + '/MMD/StoreComments',
        type: "POST",
        traditional: true,
        data: { 'ReqNo': reqNo, 'comment': comm },
        success: function (result) {
            if (result.status == true) {
                bootbox.alert({
                    message: result.msg,
                    size: "small",
                    callback: function () {
                        var dialog = bootbox.dialog({
                            message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp;Reloading..</p>',
                            closeButton: false
                        });
                        window.location.reload(true);
                    }
                });
            }
        },
        error: function () {
        },
    });
}

