$('#DiagnosticFile').change(function () {
    var file = $(this).get(0).files[0];
    if (file.type != "application/pdf") {
        $(this).val('');
        $.notify("Unable to attach the selected file. Please attach PDF file only.", { position: "bottom right", className: "error" });
    }
    else if (typeof (file) != "undefined") {
        getBase64(file);
    } else {
        $('#base64Str').val('');
    }
});

function getBase64(file) {
    var dialog = bootbox.dialog({
        message: '<p class="text-center"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>&nbsp;&nbsp; Attaching file. Please wait a moment.</p>',
        closeButton: false
    });

    var holder = $('#base64Str');
    $('#FileHolder').removeAttr('src');
    var reader = new FileReader();
    var x = reader.readAsDataURL(file);
    var count = 0;
    setTimeout(function () {
        holder.val(reader.result);
        dialog.modal('hide');
        if (holder.val() == "") {
            console.log('Not attached.');
            $('#DiagnosticFile').val('');
            console.log('Clearing diagnostic input field.');
            $.notify("Unable to attached diagnostic. Please try again", {
                position: "bottom right",
                className: "error"
            });
        } else {
            console.log('Attached successfully.');
            $.notify("File successfully attached.", {
                position: "bottom right",
                className: "success"
            });
        }
    }, 3000);
}

$(".selectRT").change(function () {
    var selected = $(".selectRT option:selected").val();
    if (selected.trim() === 'ORS-1716-00001' || selected.trim() === 'ORS-L1716-00001') {
        $("#diagnostic").fadeIn('slow');
    } else {
        $("#diagnostic").fadeOut('slow');
    }
});