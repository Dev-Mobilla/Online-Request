$(document).ready(function () {
    var inputs = $('input[name="pricePerItem"]')
    var finalPrice
    var overallTotalPrice = 0;
    inputs.each(function () {
        if ($(this).val() != "" || $(this).val() != 'undefined' || $(this).val() != 'NaN') {
            overallTotalPrice += parseFloat(document.getElementById(this.id).value.replace(/,/g, "")) || 0;
            finalPrice = overallTotalPrice.toFixed(2);
        }
    })

    finalPrice = finalPrice == "0.00" ? "" : finalPrice;

    if (typeof finalPrice != "undefined") {
        document.getElementById("overallPrice").value = finalPrice.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        document.getElementById("_overallPrice").value = finalPrice;
    }
});

function addPriceModal(selectedQty, selectedPrice) {
    document.getElementById("qtySelected").value = selectedQty;
    document.getElementById("itemSelected").value = selectedPrice;

    //console.log(quantity);
    //console.log(selectedPrice);
    $('#_AddItemPrice').modal('show');
}


function getSelectedPrice(e) {
    var selectedPrice = document.getElementById("itemSelected").value;
    var selectedQty = document.getElementById("qtySelected").value;
    var quantity = document.getElementById(selectedQty).value;
    var selectedItemPrice = document.getElementById("Price" + e).innerText;

    var price = parseFloat(selectedItemPrice);
    var totalQty = parseInt(quantity);
    var totalPrice = price * totalQty;
    var finalTotalPrice = totalPrice.toFixed(2);
    var formatPrice = finalTotalPrice.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    //console.log(price);
    //console.log(quantity);
    //console.log("quantity:", totalQty);
    //console.log(finalTotalPrice);

    document.getElementById(selectedPrice).value = formatPrice;

    $('#_AddItemPrice').modal('hide');
    $('#itemSearch').val('');
    $('#itemInfo').empty();

    function calculatePrice() {
        var finalPrice
        var overallTotalPrice = 0;
        $('.priceTotal').each(function () {
            var val = $(this).val().replace(/,/g, '');
            if (val != "" || val != 'undefined' || val != 'NaN') {
                overallTotalPrice += parseFloat(val) || 0;
                finalPrice = overallTotalPrice.toFixed(2);

            }
        })

        var formatPrice = finalPrice.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        document.getElementById("overallPrice").value = formatPrice;
        document.getElementById("_overallPrice").value = finalPrice;
    }

    calculatePrice();
}


$('#btnSearchID').click(function (e) {
    var searchItem = $('#itemSearch').val();

    if (searchItem.length != 0) {
        $.ajax({
            type: "GET",
            url: Url + '/MMD/SearchItemPrice',
            data: { searchCriteria: searchItem },
            success: function (result) {
                var resultItems = [];
                items = result.data.ListOfItems;
                if (result.status == true && items.length != 0) {
                    $.each(items, function (index, item) {
                        resultItems.push(`<tr key=${index}>
                            <td><button onclick=getSelectedPrice('${index}')>Select</button></td>
                            <td>${item.ItemCode}</td>
                            <td>${item.ItemDescription}</td>
                            <td id=Price${index}>${item.ItemPrice}</td>
                        </tr>`)
                    })
                    $('#itemInfo').empty().append(resultItems);

                } else if (items.length == 0) {
                    $('#itemInfo').empty().append('<tr data-no-results-found style="text-align:center"><td colspan="6">NO RESULTS FOUND</td></tr>');
                }
            },
            error: function () {
                bootbox.alert("Unable to process request")
            }
        });
    }
    else {
        bootbox.alert("Please input item code or description!");
    }
});


$('.priceTotal').blur(function (e) {
    var priceId = this.id;
    if ($(priceId).val() != "" || typeof $(priceId).val() === 'undefined' || $(priceId).val() == 'NaN') {

        var price = parseFloat(document.getElementById(priceId).value.replace(/,/g, ""));
        //console.log(price);
        if (isNaN(price)) {
            return;
        }

        var finalvalue = addDecimals(price.toFixed(2));

        return $(this).val(finalvalue);
    }

});

//$('.priceTotal').on('blur', function () {
//    // Get the input value
//    var inputValue = $(this).val();

//    // Convert the input value to a number
//    var numberValue = parseFloat(inputValue).toFixed(2);
//    if (isNaN(numberValue) || inputValue === '') {
//        numberValue = '';
//    }
//    console.log("Ni trigger ang BLURRR");

//    // Update the input value with the number value
//    $(this).val(numberValue.replace(/\B(?=(\d{3})+(?!\d))/g, ","));

//});


function addDecimals(value) {
    if (value === "") {
        return value = "";
    }
    var noDecimal;
    if (value.includes(',')) {
        noDecimal = value.replace(',', "");
    }
    else {
        noDecimal = value;
    }

    var number = parseFloat(noDecimal);
    var addDecimal = number.toFixed(2);
    var format = addDecimal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return format;
}


$('.priceTotal').on('change', function () {
    var finalPrice
    var overallTotalPrice = 0;
    $('.priceTotal').each(function () {
        var val = $(this).val().replace(/,/g, '');
        if (val != "" || val != 'undefined' || val != 'NaN') {
            overallTotalPrice += parseFloat(val) || 0;
            finalPrice = overallTotalPrice.toFixed(2);
        }
    })
    /*    console.log("Ni trigger ang onchange");*/

    var formatPrice = finalPrice.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    document.getElementById("overallPrice").value = formatPrice;
    document.getElementById("_overallPrice").value = finalPrice;
})


//Allow numbers and decimal point only in price input
$('.priceTotal').on('keypress', function (event) {
    // Get the entered key code
    var keycode = event.which;

    // Allow numbers and decimal point
    if ((keycode >= 48 && keycode <= 57) || keycode == 46) {
        return true;
    }

    // Disallow all other keys
    return false;
})



$('input[name="pricePerItem"]').on('input', function () {
    // Get the input value
    var inputValue = $(this).val();

    // Validate the input using a regular expression
    var decimalRegex = /^(\d+)?(\.\d{0,2})?$/;
    if (!decimalRegex.test(inputValue)) {
        // If the input value is not valid, remove the last character
        $(this).val(inputValue.slice(0, -1));
    }
});




