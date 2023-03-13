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
    var comm = encodeURIComponent($('#commTextArea').val());

    $.ajax({
        url: Url + '/MMD/StoreComments',
        type: "POST",
        traditional: true,
        data: { 'ReqNo': reqNo, 'comment': comm },
        success: function (result) {
            if (result.status == true) {
                bootbox.dialog({
                    message: "<h5><center>" + result.msg + "</center></h5>",
                    size: "small",
                    closeButton: false
                });

                window.setTimeout(function () {
                    bootbox.hideAll();
                    window.location.reload(true);

                }, 1000);
            }
        },
        error: function () {
        },
    });
}

