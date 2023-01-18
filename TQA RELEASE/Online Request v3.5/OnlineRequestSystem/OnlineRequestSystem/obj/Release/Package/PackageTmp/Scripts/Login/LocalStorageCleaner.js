$('#ss').click(function () {
    alert($('.s').val());
});
localStorage.clear();

$('#xLoader').show();
$(document).ready(function () {
    setTimeout(function () {
        $("#xLoader").hide('drop', { direction: 'up' }, 'fast', function () {
            $('#xForm').show('fade', 'fast');
        });
    }, 1200);
});