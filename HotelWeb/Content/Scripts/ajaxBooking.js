/// <reference path="../Scripts/jquery-2.1.1.js" />
var startDate, nights;
var amountOfDatesInTable = 7;               //show one week in the table

$(document).ready(function () {
    $("#baseInfoSubmit").click(function (event) {
        event.preventDefault();

        //create data object from form
        var baseData = {
            StartDate: $("#bookingstart").val(),
            Nights: $("#bookingnights").val(),
            PersonRoom: $("#personen").val(),
            BiggerRoom: $("#bookingmorepermitted").prop("checked")
        }

        //check data
        try{
            checkAlert(baseData.StartDate == "" ? "Check de startdatum!" : "");
            console.log("Zie je wel :  " + baseData.Nights);
            checkAlert(baseData.Nights < 1 ? "We verwachten dat je 1 of meer nachten komt slapen. Check het aantal nachten!" : "");
        } catch (e) {
            return;
        }

        //asign correct values to globals
        startDate = baseData.StartDate;
        nights = baseData.Nights

        //log collected data
        console.log(baseData);

        //sent data to server to get header information
        $.ajax(
        {
            url: "/Book/test",
            data: baseData,
            type: "post",
            success: function (headData) {
                $("#select-booking-date").slideDown();
                resetBookingTable(true);

                addBookingTableHeader(headData);
                changeBookingData();

                $("#booking-table-container").slideDown();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("An error occured while trying to collect data: " + thrownError);
            }
        });
    });
});

function resetBookingTable(removeHeader) {
    $tablecontainer = $("#booking-table-container");
    $tablecontainer.slideUp().find("tbody tr").remove();

    if (removeHeader) {
        $tablecontainer.find("thead tr").remove();
    }
}

function addBookingTableHeader(headerJson) {
    var headerObj = jQuery.parseJSON(headerJson);
    $("#booking-table thead").append("<tr></tr>");

    var $bookingTableHeader = $("#booking-table thead tr").first();
    $bookingTableHeader.append("<th>Data</th>");

    headerObj.forEach(function (room) {
        $bookingTableHeader.append("<th data-id=\"" + room.ID +
            "\">Kamer " + room.ID + "<br />" + room.NumberOfPersons +
            " persoons<br />Vanaf &euro;" + room.MinPrice + ".-</th>");
        console.log(room);
    });

}

function changeBookingData() {
    console.log("You filling table with " + startDate + " for items " + amountOfDatesInTable);
}

function checkAlert(data) {
    if (data == "" || data == null) {
        return;
    } else {
        alert(data);
        throw "Sorry, you have to check your input";
    }
}