$(document).ready(function () {
    //jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    //    "date-uk-pre": function (a) {
    //        console.log(a)
    //        var ukDatea = a.split('/');
    //        return (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
    //    },

    //    "date-uk-asc": function (a, b) {          
    //        return ((a < b) ? -1 : ((a > b) ? 1 : 0));
    //    },

    //    "date-uk-desc": function (a, b) {
    //        return ((a < b) ? 1 : ((a > b) ? -1 : 0));
    //    }
    //});

    $('#BMorOthers').DataTable({
        "order": [[2, "asc"]], "bSort": false, stateSave: true
    });
    $('#AMOpen').DataTable({
        "order": [[2, "asc"]], "bSort": false, stateSave: true
    });
    $('#DeptOpenReq').DataTable({
        "order": [[2, "asc"]], "bSort": false, stateSave: true
    });
    $('#RMOpen').DataTable({
        "order": [[2, "asc"]], "bSort": false, stateSave: true
    });

    $('#ReqI').DataTable({
        "order": [[1, "asc"]], "bSort": false, stateSave: true
    });
    $('#ReqII').DataTable({
        stateSave: true,
        "order": [[1, "asc"]],
        columns: [
            { orderable: false },
            null,null, null, null, null, null, null, null
        ],
    });

    $('#DivisionOpenReq').DataTable({
        stateSave: true,
        "order": [[2, "asc"]],
        "aoColumns": [
            { orderable: false },
            { orderable: false },
            null, null, null, null, null, null, null, null
        ]
    });

    $('#GMOpen').DataTable({
        stateSave: true,
        "order": [[2, "asc"]],
        columns: [
            { orderable: false },
            { orderable: false },
            null, null, null, null, null, null, null, null
        ]
    });
    $('#PresidentOpen').DataTable({
        stateSave: true,
        "order": [[2, "asc"]],
        columns: [
            { orderable: false },
            { orderable: false },
            null, null, null, null, null, null, null, null
        ]
    });

    $('#MMDOpenRequest').DataTable({
        stateSave: true,
        "order": [[1, "asc"]],
        columns: [
            { orderable: false },
            null, null, null, null, null, null, null, null
        ],

    });

    $(".requests").DataTable({
        //"dom": 'lrtip',
        /*"order": [[0, "asc"]],*/
        /*"bSort": false,*/
        "order": [[2, "asc"]],
        columns: [
            { orderable: false },
            { orderable: false },
            null, null, null, null, null, null, null, null, null,
            { orderable: false }
        ],
        stateSave: true,
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                var priceVal = i.toString().replace(/,/g, '');
                let val = priceVal === '' && typeof priceVal === 'string' ? 0 : parseFloat(priceVal);
                return val
            };

            var tbl = $('.requests th')
            var index;
            tbl.each(function () {
                if ($(this).text() == "Price") {
                    tbl.index(this);
                    //console.log(tbl.index(this));
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
            $(api.column(index).footer()).html(total == 0.00 ? '' : total.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        },
    });

    $(".requestsMMD").DataTable({
        //"dom": 'lrtip',
        /*"order": [[0, "asc"]],*/
        /*"bSort": false,*/
        "order": [[1, "asc"]],
        columns: [
            { orderable: false },
            null, null, null, null, null, null, null, null, null,
            { orderable: false }
        ],
        stateSave: true,
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                var priceVal = i.toString().replace(/,/g, '');
                let val = priceVal === '' && typeof priceVal === 'string' ? 0 : parseFloat(priceVal);
                return val
            };

            var tbl = $('.requestsMMD th')
            var index;
            tbl.each(function () {
                if ($(this).text() == "Price") {
                    tbl.index(this);
                    //console.log(tbl.index(this));
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
            $(api.column(index).footer()).html(total == 0.00 ? '' : total.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        },
    });
});

$(".table_link").on("click", function () {
    $('#MMDOpenRequest,#BMorOthers,#AMOpen,#DeptOpenReq,#DivisionOpenReq,#GMOpen,#PresidentOpen,#RMOpen').DataTable().state.clear();
});