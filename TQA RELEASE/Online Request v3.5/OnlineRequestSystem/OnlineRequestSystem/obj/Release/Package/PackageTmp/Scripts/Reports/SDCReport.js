$(document).ready(function () {
    $("#sdcPickDate").datepicker({
        maxDate: new Date()
    });
});

$('#Tabb').on('click', function () {
    $('.Dp').prop('selectedIndex', 0);
});

$(function () {
    $('.date-picker').datepicker({
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'MM yy',
        onClose: function (dateText, inst) {
            $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth, 1));
        }
    });
});