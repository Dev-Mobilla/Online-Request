var urlLink = "";
//var urlLink = "/CustomerTrend";

$(document).ready(function () {
    idleTime = 0;
    $(document).ready(function () {
        $limit = 900;
        function timerIncrement() {
            idleTime = idleTime + 1;

            if (idleTime > $limit) {
                $('.modal-inactivity').modal('show');
                $.ajax({
                    url: urlLink + '/Login/Login',
                    success: function () { }, error: function () { }
                });
                idleTime = 0;
            }
        }
        // Increment the idle time counter every second.
        var idleInterval = setInterval(timerIncrement, 1000);

        // Zero the idle timer on mouse movement.
        $(this).mousemove(function (e) {
            idleTime = 0;
        });
        $(this).keypress(function (e) {
            idleTime = 0;
        });
    });
});