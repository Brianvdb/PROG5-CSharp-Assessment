/// <reference path="../Scripts/jquery-2.1.1.js" />
//alert($("#different-invoice-adress-checkbox").prop("checked") );
$(document).ready(function () {
    $("#different-invoice-adress-checkbox").change(function () {
        $("#different-invoice-adress").toggle("slow");
    });

    //get prices
    $.ajax({
        type: "POST",
        url: '/Book/GetPrices',
        dataType: "json",
        success: function (data) {
            console.log(data);     
            var dataObj = jQuery.parseJSON(data);
            console.log(dataObj);
            
            var priceList = "";
            var totalPrice = 0;
            dataObj.forEach(function (e) {
                totalPrice += e.price
                priceList += "<tr><td>" + e.date + "</td><td>&euro;" + e.price + "</td></tr>";
            });
            priceList += "<tr style=\"border-top: 2px solid black\"><td>Totaalprijs:</td><td>&euro;" + totalPrice.toFixed(2) + "</td></tr>"
            $("#price-tbody").append(priceList);
        },
        error: function (data) {
            alert("Excuses, we hebben de prijzen niet van de server op kunnen halen. Probeer de boeking opnieuw.");
        }
    });
});