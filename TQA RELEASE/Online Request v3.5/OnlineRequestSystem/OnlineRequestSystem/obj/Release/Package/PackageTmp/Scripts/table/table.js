$(document).ready(function () {
    $('#table_data').DataTable({
        "order": [[0, "desc"]]
    });

    $('#table_dataV2').DataTable({
        "ordering": false,
        "searching": false,
        "order": [[0, "desc"]]
    });

    $('#table_dataV3').DataTable({
        "ordering": false,
        "order": [[2, "desc"]]
    });
});