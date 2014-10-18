/// <reference path="../Scripts/jquery-2.1.1.js" />
$(document).ready(function () {
    $("#baseInfoSubmit").click(function (event) {
        var baseData = {
            StartDate: $("#bookingstart").val(),
            Nights: $("#bookingnights").val(),
            PersonRoom: $("#personen").val(),
            BiggerRoom: $("#bookingmorepermitted").prop("checked")
        }

        console.log(baseData);

        event.preventDefault();
        $.ajax(
        {
            url: "/Book/test",
            data: baseData,
            type: "post",
            success: function (msg) {
                console.log(msg);
                console.log("Was here!");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Could not add the item to your cart: " + thrownError);
            }
        });

        alert("EOL");
    });
});