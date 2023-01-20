// MMD
$(".DivOptions").on('click', function () {
    var Status = $(this).text().trim();
    var lastChild = $(this).closest("tr").find('td:last-child');
    if ($(this).closest("tr").find('td:last-child').text().trim() == 'Cancelled') {
        $.notify("Cannot update status if item is already cancelled.", { position: "bottom right", className: "error" });
    } else {
        var Div = $(this).closest('tr').find('.DivSelect');
        var DivStatus = Div.attr('class');
        var xDivStatus = DivStatus.replace('dropdown-toggle DivSelect', '').trim();

        var newClass = classSelector(Status.trim());
        switch (xDivStatus) {
            case 'fa fa-caret-square-o-down': //Open
                $(this).closest('td').find('.DivSelect').removeClass(xDivStatus)
                $(this).closest('td').find('.DivSelect').addClass(newClass);
                break;
            case 'fa fa-check-circle txtSuccess': //Servde
                $(this).closest('td').find('.DivSelect').removeClass(xDivStatus)
                $(this).closest('td').find('.DivSelect').addClass(newClass);
                break;
            case 'fa fa-share-square-o txtWarning': // Pending
                $(this).closest('td').find('.DivSelect').removeClass(xDivStatus)
                $(this).closest('td').find('.DivSelect').addClass(newClass);
                break;
            case 'fa fa-window-close txtDanger': // Cancelled
                $(this).closest('td').find('.DivSelect').removeClass(xDivStatus)
                $(this).closest('td').find('.DivSelect').addClass(newClass);
                break;
            default:
        }

        var Description = $(this).closest("tr").find('td:eq(1)').text();
        var Desc = Description.trim();

        $.ajax({
            type: "GET",
            url: Url + '/RequestItems/DivItemStatus',
            data: {
                ReqNo: ReqNO,
                status: Status,
                description: Desc
            },
            success: function (result) {
                if (result.status === true) {
                    Div.removeClass(xDivStatus);
                    Div.addClass(classSelector(Status));
                    lastChild.html(Status);
                    $.notify(Desc + " : " + Status, { position: "bottom right", className: "success" });
                } else {
                    
                    $.notify("Unable to process request.", { position: "bottom right", className: "error" });
                }
            },
            error: function () {
                $.notify("Unable to process request.", { position: "bottom right", className: "error" });
            }
        });
    }
});

function classSelector(Status) {
    switch (Status) {
        case 'Open':
            return 'fa fa-caret-square-o-down';
            break;
        case 'Served':
            return 'fa fa-check-circle txtSuccess';
            break;
        case 'Pending':
            return 'fa fa-share-square-o txtWarning';
            break;
        case 'Cancelled':
            return 'fa fa-window-close txtDanger';
            break;
        default:
    }
}