var minDate, maxDate;

Date.prototype.addHours = function (h) {
    this.setTime(this.getTime() + (h * 60 * 60 * 1000));
    return this;
}

// Custom filtering function which will search data in column four between two values
$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var min = minDate.val();
        var max = maxDate.val();
        var date = new Date(data[2]).addHours(8);
        var dataStatus = data[0];
        var stat = document.getElementById("filter_Status").value;

        if (stat === "All Requests") {
            if (
                (min === null && max === null) ||
                (min === null && date <= max) ||
                (min <= date && max === null) ||
                (min <= date && date <= max)
            ) {
                return true;
            }
        }

        else if (stat === "Open Requests" && dataStatus === stat) {
            if (
                (min === null && max === null) ||
                (min === null && date <= max) ||
                (min <= date && max === null) ||
                (min <= date && date <= max)
            ) {
                return true;
            }
        }

        else if (stat === "PO Approval" && dataStatus === stat) {
            if (
                (min === null && max === null) ||
                (min === null && date <= max) ||
                (min <= date && max === null) ||
                (min <= date && date <= max)
            ) {
                return true;
            }
        }

        return false;
    }
);

$(document).ready(function () {

    // Create date inputs

    $('#min, #max').on('change', function () {

        if ($('#min').val() == "" || $('#min').val() == null) {
            minDate = new DateTime($('#min'), {
                format: 'MM/DD/YYYY'

            });
        }

        if ($('#max').val() == "" || $('#max').val() == null) {
            maxDate = new DateTime($('#max'), {
                format: 'MM/DD/YYYY'

            });
        }
    });

    if ($('#min').val() == "" || $('#min').val() == null) {
        minDate = new DateTime($('#min'), {
            format: 'MM/DD/YYYY'

        });
    }

    if ($('#max').val() == "" || $('#max').val() == null) {
        maxDate = new DateTime($('#max'), {
            format: 'MM/DD/YYYY'

        });
    }

    // DataTables initialisation
    var table = $('#MMDOpenRequest').DataTable({
        "order": [[0, "desc"]], "bSort": false, stateSave: true,

    });

    $('#filter_Status').on('change', function () {
        table.draw();
    });

    // Refilter the table
    $('#search_filter').on('click', function () {
        table.draw();
    });
});