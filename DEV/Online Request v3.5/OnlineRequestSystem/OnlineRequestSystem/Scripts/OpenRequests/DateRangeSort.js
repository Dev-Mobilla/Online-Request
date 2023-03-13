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
