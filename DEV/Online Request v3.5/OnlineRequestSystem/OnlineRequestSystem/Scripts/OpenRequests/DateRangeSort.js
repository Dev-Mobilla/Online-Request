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

    //console.log(getUser);

//     if (getUser == "Pres" || getUser == "VPO" || getUser == "MMD" || getUser == "VPO_local") {
//         custom_search();
//     }

//     function custom_search() {
//         $.fn.dataTable.ext.search.push(
//             function (settings, data, dataIndex) {

//                 var index;

//                 if ($('#forFilter').val() == "Pres" || $('#forFilter').val() == "VPO") {
//                     index = 2;
//                 }
//                 else {
//                     index = 1;
//                 }

//                 var min = minDate.val();
//                 var max = maxDate.val();
//                 var date = new Date(data[index]).addHours(8);
//                 var dataStatus = data[13];
//                 var stat = document.getElementById("filter_Status").value;

//                 if (stat === "All Requests") {
//                     if (
//                         (min === null && max === null) ||
//                         (min === null && date <= max) ||
//                         (min <= date && max === null) ||
//                         (min <= date && date <= max)
//                     ) {
//                         return true;
//                     }
//                 }

//                 else if (stat === "Open Requests" && dataStatus === stat) {
//                     if (
//                         (min === null && max === null) ||
//                         (min === null && date <= max) ||
//                         (min <= date && max === null) ||
//                         (min <= date && date <= max)
//                     ) {
//                         return true;
//                     }
//                 }

//                 else if (stat === "PO Approval" && dataStatus === stat) {
//                     if (
//                         (min === null && max === null) ||
//                         (min === null && date <= max) ||
//                         (min <= date && max === null) ||
//                         (min <= date && date <= max)
//                     ) {
//                         return true;
//                     }
//                 }

//                 return false;
//             }
//         );
//     }

// });
