$(document).ready(function () {
    $("#pickdateFrom").datepicker({
        maxDate: new Date()
    });
    $("#pickdateTo").datepicker({
        maxDate: new Date()
    });
});

$("#pickdateFrom").on('click', function () {
    $(this).datepicker('show');
});

$("#pickdateTo").on('click', function () {
    $(this).datepicker('show');
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