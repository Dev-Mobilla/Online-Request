
function addPriceModal(selectedQty, selectedPrice) {
    document.getElementById("qtySelected").value = selectedQty;
    var quantity = document.getElementById(selectedQty).innerText;
    document.getElementById("itemSelected").value = selectedPrice;

    //console.log(quantity);
    //console.log(selectedPrice);
    $('#_AddItemPrice').modal('show');
}


function getSelectedPrice(e) {
    var selectedPrice = document.getElementById("itemSelected").value;
    var selectedQty = document.getElementById("qtySelected").value;
    var quantity = document.getElementById(selectedQty).innerText;
    var selectedItemPrice = document.getElementById("Price" + e).innerText;

    var price = parseFloat(selectedItemPrice);
    var totalQty = parseInt(quantity);
    var totalPrice = price * totalQty;
    var finalTotalPrice = totalPrice.toFixed(2);
    //console.log(price);
    //console.log(quantity);
    //console.log("quantity:", totalQty);
    //console.log(finalTotalPrice);

    document.getElementById(selectedPrice).value = finalTotalPrice;

    $('#_AddItemPrice').modal('hide');
    $('#itemSearch').val('');
    $('#itemInfo').empty();

    function calculatePrice() {
        var inputs = $('input[name="pricePerItem"]')
        var finalPrice

        var overallTotalPrice = 0;
        inputs.each(function () {
            if ($(this).val() != "") {
                overallTotalPrice += parseFloat($(this).val()) || 0;
                finalPrice = overallTotalPrice.toFixed(2);
            }
        })

        document.getElementById("overallPrice").value = finalPrice
        document.getElementById("_overallPrice").value = finalPrice;
    }

    calculatePrice();
}

var inputs = $('input[name="pricePerItem"]')
inputs.change(function () {
    var finalPrice
    var overallTotalPrice = 0;
    inputs.each(function () {
        if ($(this).val() != "" || $(this).val() != 'undefined') {
            overallTotalPrice += parseFloat($(this).val()) || 0;
            finalPrice = overallTotalPrice.toFixed(2);
        }
    })

    document.getElementById("overallPrice").value = finalPrice;
    document.getElementById("_overallPrice").value = finalPrice;
})


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

//Allow decimal numbers
//function isNumberKey(evt, element) {
//    var charCode = (evt.which) ? evt.which : event.keyCode
//    if (charCode > 31 && (charCode < 48 || charCode > 57) && !(charCode == 46 || charCode == 8)) {

//        return false;
//    }

//    else {
        
//        var len = $(element).val().length;
//        var index = $(element).val().indexOf('.');
//        if (index > 0 && charCode == 46) {
            
//            return false;
//        }
//        if (index > 0) {

//            var CharAfterdot = (len + 1) - index;

//            if (CharAfterdot > 3) {
//                if (element.val().includes(".") &&) {
//                    return true;
//                }
//                console.log("Ari ni sya ni ungot")
//                return false;
//            }
//        }

//    }
//    return true;
//}

//function onkeydownAmount(e) {

//    let invalidInputs = ["e", "E", "+", "-"];
//    let numbers = (/^[-+]?[0-9]*\.?[0-9]*$/.test(e.key))


//    let isInclude = invalidInputs.includes(e.key);

//    if (isInclude) {
//        return e.preventDefault();
//    } else if (numbers || e.key == "Backspace") {
//        //console.log(e)
//        return e
//    } else {
//        return e.preventDefault();
//    }

//}

//function onChangePrice(e) {
//    console.log(e)
//    var val = parseFloat(e.target.value).toFixed(2);
//     document.getElementById(e.target.id).value = val
//    //document.getElementById(e.target.id).value = val.toLocaleString("en", { useGrouping: true, maximumFractionDigits: 2, minimumFractionDigits: 2 })
//}

$(".priceTotal").blur(function (e) {
    
    var price = parseFloat(document.getElementsByClassName("priceTotal").value.replace(/,/g, ""));

    e.preventDefault()
        
    var finalvalue = addDecimals(price.toFixed(2));
    return $(this).val(finalvalue);
   
});

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
    var value = $(this).val();
    var finalValue = addDecimals(value);
    return $(this).val(finalValue);
})











