$('#BMorOthers').DataTable({
    "order": [[2, "desc"]], "bSort": false, stateSave: true
});
$('#AMOpen').DataTable({
    "order": [[2, "desc"]], "bSort": false, stateSave: true
});
$('#DeptOpenReq').DataTable({
    "order": [[2, "desc"]], "bSort": false, stateSave: true
});

$('#ReqI').DataTable({
    "order": [[2, "desc"]], "bSort": false, stateSave: true
});
$('#ReqII').DataTable({
    stateSave: true,
    "order": [[2, "desc"]],
    columns: [
        { orderable: false },
        null, null, null, null, null, null, null,null
    ]
});

$('#DivisionOpenReq').DataTable({
    stateSave: true,
    "order": [[2, "desc"]],
    columns: [
        { orderable: false },
        { orderable: false }, null, null, null, null, null, null, null, null
    ]
});
$('#MMDOpenRequest').DataTable({
    stateSave: true,
    "order": [[1, "desc"]],
    columns: [
        { orderable: false },
        null, null, null, null, null, null, null, null
    ]
});
$('#GMOpen').DataTable({
    stateSave: true,
    "order": [[2, "desc"]],
    columns: [
        { orderable: false },
        { orderable: false }, null, null, null, null, null, null, null, null
    ]
});
$('#PresidentOpen').DataTable({
    stateSave: true,
    "order": [[2, "desc"]],
    columns: [
        { orderable: false },
        { orderable: false }, null, null, null, null, null, null, null, null
    ]
});
$('#RMOpen').DataTable({
    "order": [[2, "desc"]], "bSort": false, stateSave: true
});

$(".table_link").on("click", function () {
    $('#MMDOpenRequest,#BMorOthers,#AMOpen,#DeptOpenReq,#DivisionOpenReq,#GMOpen,#PresidentOpen,#RMOpen').DataTable().state.clear();
});