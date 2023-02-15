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

        //console.log(min);

        //console.log(date);

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

    var sortID = $('#usr_For_sort').val();

    var sorIDD = "'#" + sortID + "'";

    console.log(sorIDD);

     //DataTables initialisation
    //var table = $("#" + sortID).DataTable({
    //    "order": [[0, "desc"]], "bSort": false, stateSave: true, "bDestroy": true,
        
    //});
    

    $('#filter_Status').on('change', function () {
        table.draw();
    });

    // Refilter the tables
    $('#search_filter').on('click', function () {
        table.draw();
    });


    $(".requests").DataTable({
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
                    index =  tbl.index(this);
                }
            })
            console.log(index)
            


            // Total over all pages
            total = api
                .column(index)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            // Update footer
            console.log(total)
            $(api.column(index).footer()).html(total.toFixed(2));
        },
    });

 
});