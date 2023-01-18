//$(document).ready(function () {
//    $(" .").bind('paste', function (e) {
//            var data = e.originalEvent.clipboardData.getData('Text');

//            if (hasInvalidChars(data) == true) {
//                return false;
//            }
//            else {
//                return true;
//            }
//        });
//});

function hasInvalidChars(s) {
    var validChars = /^[a-zA-Z0-9\-\s]+/;
    var x = s.replace(validChars, '');
    return !!x.length;
}
function AlphaNumeric() {
    var regex = /^[0-9]+/;
    var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
    if (!regex.test(key)) {
        event.preventDefault();
        return false;
    }
}