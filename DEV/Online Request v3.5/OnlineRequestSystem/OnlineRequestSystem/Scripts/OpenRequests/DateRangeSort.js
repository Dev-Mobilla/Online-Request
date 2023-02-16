var minDate, maxDate;

Date.prototype.addHours = function (h) {
    this.setTime(this.getTime() + (h * 60 * 60 * 1000));
    return this;
}

// Custom filtering function which will search data in column four between two values
$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {

        var index;

        if ($('#forFilter').val() == "Pres" || $('#forFilter').val() == "VPO") {
            index = 2;
        }
        else {
            index = 1;
        }

        var min = minDate.val();
        var max = maxDate.val();
        var date = new Date(data[index]).addHours(8);
        var dataStatus = data[13];
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

    //var sortID = $('#usr_For_sort').val();

    //var sorIDD = "'#" + sortID + "'";

    //DataTables initialisation
    //var table = $("#" + sortID).DataTable({
    //    "order": [[0, "desc"]], "bSort": false, stateSave: true, "bDestroy": true,

    //});

    var table = $(".requests").DataTable({
        "order": [[0, "desc"]],
        "bSort": false,
        stateSave: true,
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                let val = i === '' && typeof i === 'string' ? 0 : parseFloat(i);
                return val
            };

            var tbl = $('.requests th')
            var index;
            tbl.each(function () {
                if ($(this).text() == "Price") {
                    tbl.index(this);
                    console.log(tbl.index(this));
                    index = tbl.index(this);
                }
            })

            // Total over all pages
            total = api
                .column(index)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            // Update footer
            $(api.column(index).footer()).html(total.toFixed(2));
        },
    });

    $('#filter_Status').on('change', function () {
        table.draw();
    });

    // Refilter the tables
    $('#search_filter').on('click', function () {
        table.draw();
    });

});