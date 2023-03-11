// MMD
$(".BranchOptions").on('click', function () {
    var index;
    if ($('#isRequest').val() == "PO") {
        index = 2;
    } else {
        index = 1;
    }

    var Status = $(this).text().trim();
    var lastChild = $(this).closest("tr").find('td:nth-last-child(' + index + ')');
    if ($(this).closest("tr").find('td:nth-last-child(' + index + ')').text().trim() == 'Cancelled') {
        $.notify("Cannot update status if item is already cancelled.", { position: "bottom right", className: "error" });
    } else {
        var Branch = $(this).closest('tr').find('.BranchSelect');
        var BranchStatus = Branch.attr('class');
        var xBranchStatus = BranchStatus.replace('dropdown-toggle BranchSelect', '').trim();

        var newClass = classSelector(Status.trim());
        switch (xBranchStatus) {
            case 'fa fa-caret-square-o-down': //Open
                $(this).closest('td').find('.BranchSelect').removeClass(xBranchStatus)
                $(this).closest('td').find('.BranchSelect').addClass(newClass);
                break;
            case 'fa fa-check-circle txtSuccess': //Served
                $(this).closest('td').find('.BranchSelect').removeClass(xBranchStatus)
                $(this).closest('td').find('.BranchSelect').addClass(newClass);
                break;
            case 'fa fa-share-square-o txtWarning': // Pending
                $(this).closest('td').find('.BranchSelect').removeClass(xBranchStatus)
                $(this).closest('td').find('.BranchSelect').addClass(newClass);
                break;
            case 'fa fa-window-close txtDanger': // Cancelled
                $(this).closest('td').find('.BranchSelect').removeClass(xBranchStatus)
                $(this).closest('td').find('.BranchSelect').addClass(newClass);
                break;
            default:
        }

        var Description = $(this).closest("tr").find('td:eq(1)').text();
        var Desc = Description.trim();

        $.ajax({
            type: "GET",
            url: Url + '/RequestItems/BranchItemStatus',
            data: { ReqNo: ReqNO, status: Status, description: Desc},
            success: function (result) {
                if (result.status === true) {
                    Branch.removeClass(xBranchStatus);
                    Branch.addClass(classSelector(Status));
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