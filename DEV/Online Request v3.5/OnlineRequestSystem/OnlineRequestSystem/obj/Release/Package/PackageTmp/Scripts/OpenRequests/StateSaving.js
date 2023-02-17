$('#BMorOthers').DataTable({
    "order": [[0, "desc"]], "bSort": false, stateSave: true
});
$('#AMOpen').DataTable({
    "order": [[0, "desc"]], "bSort": false, stateSave: true
});
$('#DeptOpenReq').DataTable({
    "order": [[0, "desc"]], "bSort": false, stateSave: true
});
$('#DivisionOpenReq').DataTable({
    "order": [[0, "desc"]], "bSort": false, stateSave: true
});
////$('#GMOpen').DataTable({
//    "order": [[0, "desc"]], "bSort": false, stateSave: true
//});
//$('#PresidentOpen').DataTable({
//    "order": [[0, "desc"]], "bSort": false, stateSave: true
//});
$('#RMOpen').DataTable({
    "order": [[0, "desc"]], "bSort": false, stateSave: true
});

$(".table_link").on("click", function () {
    $('#MMDOpenRequest,#BMorOthers,#AMOpen,#DeptOpenReq,#DivisionOpenReq,#GMOpen,#PresidentOpen,#RMOpen').DataTable().state.clear();
});