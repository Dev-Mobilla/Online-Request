// MMD
$(".MMDOptions").on('click', function () {
    var Status = $(this).text().trim();
    if (isDivRequest == 1) {
        var Div = $(this).closest('tr').find('.DivSelect');
        var DivStatus = Div.attr('class');
        var xDivStatus = DivStatus.replace('dropdown-toggle DivSelect', '').trim();
    } else {
        var Branch = $(this).closest('tr').find('.BranchSelect');
        var SDC = $(this).closest('tr').find('.SDCSelect');

        var BranchStatus = Branch.attr('class');
        var xBranchStatus = BranchStatus.replace('dropdown-toggle BranchSelect', '').trim();

        var SDCStatus = SDC.attr('class');
        var xSDCStatus = SDCStatus.replace('dropdown-toggle SDCSelect', '').trim();
    }

    var MMDStatus = $(this).closest('td').find('.MMDSelect').attr('class');
    var x = MMDStatus.replace('dropdown-toggle MMDSelect', '');
    var newClass = classSelector(Status.trim());

    console.log(newClass);

    switch (x.trim()) {
        case 'fa fa-caret-square-o-down': //Open
            $(this).closest('td').find('.MMDSelect').removeClass(x)
            $(this).closest('td').find('.MMDSelect').addClass(newClass);
            break;
        case 'fa fa-check-circle txtSuccess': //Served
            $(this).closest('td').find('.MMDSelect').removeClass(x)
            $(this).closest('td').find('.MMDSelect').addClass(newClass);
            break;
        case 'fa fa-share-square-o txtWarning': // Pending
            $(this).closest('td').find('.MMDSelect').removeClass(x)
            $(this).closest('td').find('.MMDSelect').addClass(newClass);
            break;
        case 'fa fa-window-close txtDanger': // Cancelled
            $(this).closest('td').find('.MMDSelect').removeClass(x)
            $(this).closest('td').find('.MMDSelect').addClass(newClass);
            break;
        default:
    }

    var lastChild = $(this).closest("tr").find('td:nth-last-child(2)');
    var Description = $(this).closest("tr").find('td:eq(1)').text();
    var Desc = Description.trim();

    console.log(Desc);

    $.ajax({
        type: "GET",
        url: Url + '/RequestItems/MMDItemStatus',
        data: {
            ReqNo: ReqNO,
            status: Status,
            description: Desc,
            isDivrequest: isDivRequest
        },
        success: function (result) {
            if (result.status === true) {
                if (result.classIcon == 'Cancelled') {
                    if (isDivRequest == 1) {
                        Div.removeClass(xDivStatus);
                        Div.addClass(classSelector('Cancelled'));
                        lastChild.html(Status);
                        $.notify(Desc + " : " + Status, { position: "bottom right", className: "success" });
                    } else {
                        Branch.removeClass(xBranchStatus);
                        Branch.addClass(classSelector('Cancelled'));

                        SDC.removeClass(xSDCStatus);
                        SDC.addClass(classSelector('Cancelled'));
                        lastChild.html(Status);
                        $.notify(Desc + " : " + Status, { position: "bottom right", className: "success" });
                    }
                } else {
                    if (isDivRequest == 1) {
                        Div.removeClass(xDivStatus);
                        Div.addClass(classSelector('Open'));
                        lastChild.html('Open');
                        $.notify(Desc + " : " + Status, { position: "bottom right", className: "success" });
                    } else {
                        Branch.removeClass(xBranchStatus);
                        Branch.addClass(classSelector('Open'));

                        SDC.removeClass(xSDCStatus);
                        SDC.addClass(classSelector('Open'));
                        lastChild.html('Open');
                        $.notify(Desc + " : " + Status, { position: "bottom right", className: "success" });
                    }
                }
            } else {
                $.notify("Unable to process request. Test", { position: "bottom right", className: "error" });
            }
        },
        error: function () {
            $.notify("Unable to process request. Test error", { position: "bottom right", className: "error" });
        }
    });
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