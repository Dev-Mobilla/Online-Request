$(document).ready(function () {
    $(".requests").DataTable({
        //"dom": 'lrtip',
        /*"order": [[0, "desc"]],*/
        /*"bSort": false,*/
        "order": [[2, "desc"]],
        columns: [
            { orderable: false },
            { orderable: false }, null, null, null, null, null, null, null, null, null,
            { orderable: false }
        ],
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
            $(api.column(index).footer()).html(total == 0.00 ? 0 : total.toFixed(2));
        },
    });
    $(".requestsMMD").DataTable({
        //"dom": 'lrtip',
        /*"order": [[0, "desc"]],*/
        /*"bSort": false,*/
        "order": [[1, "desc"]],
        columns: [
            { orderable: false },null, null, null, null, null, null, null, null, null,
            { orderable: false }
        ],
        stateSave: true,
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                let val = i === '' && typeof i === 'string' ? 0 : parseFloat(i);
                return val
            };

            var tbl = $('.requestsMMD th')
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
            $(api.column(index).footer()).html(total == 0.00 ? 0 : total.toFixed(2));
        },
    });

});

//var minDate, maxDate;

//Date.prototype.addHours = function (h) {
//    this.setTime(this.getTime() + (h * 60 * 60 * 1000));
//    return this;
//}