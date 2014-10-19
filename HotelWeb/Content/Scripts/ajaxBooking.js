/// <reference path="../Scripts/jquery-2.1.1.js" />
var hotelIds = [];
var nights = -1;
var startDate = 0;
$(document).ready(function () {
    $("#baseInfoSubmit").click(function (event) {
        event.preventDefault();
        hotelIds = [];
        nights = -1;
        startDate = 0;

        //create data object from form
        var baseData = {
            PersonRoom: $("#personen").val(),
            BiggerRoom: $("#bookingmorepermitted").prop("checked")
        }

        var inputStartdate = $("#bookingstart").val();
        var inputNights = $("#bookingnights").val();

        //check data
        try{
            checkAlert(inputStartdate == "" ? "Check de startdatum!" : "");
            checkAlert(inputNights < 1 ? "We verwachten dat je 1 of meer nachten komt slapen. Check het aantal nachten!" : "");
        } catch (e) {
            return;
        }

        startDate = inputStartdate;
        nights = inputNights;

        //log collected data
        console.log(baseData);

        //sent data to server to get header information
        $.ajax(
        {
            url: "/Book/Header",
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
        hotelIds.push(room.ID);
        $bookingTableHeader.append("<th data-id=\"" + room.ID +
            "\">Kamer " + room.ID + "<br />" + room.NumberOfPersons +
            " persoons<br />Vanaf &euro;" + room.MinPrice + ".-</th>");
        console.log(room);
    });

}

function changeBookingData() {
    console.log(hotelIds);
    var baseData = {
        StartDate: startDate,
        AmountOfDatesInTable: 7,
        RoomSelection:hotelIds.toString()
    }
    $.ajax(
    {
        url: "/Book/Dates",
        data: baseData,
        type: "post",
        success: function (headData) {
            console.log(headData);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("An error occured while trying to collect complex data: " + thrownError);
        }
    });
}

function checkAlert(data) {
    if (data == "" || data == null) {
        return;
    } else {
        alert(data);
        throw "Sorry, you have to check your input";
    }
}