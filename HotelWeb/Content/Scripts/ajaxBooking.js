/// <reference path="../Scripts/jquery-2.1.1.js" />
var hotelIds = [];
var nights = -1;
var startDate = 0;
var items = 7;

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
        AmountOfDatesInTable: items,
        RoomSelection:hotelIds.toString()
    }
    $.ajax(
    {
        url: "/Book/Dates",
        data: baseData,
        type: "post",
        success: function (bodyData) {
            try{
                var obj = jQuery.parseJSON(bodyData);
            }catch(e){
                var obj = null;
            }

            drawBookingData(obj);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("An error occured while trying to collect complex data: " + thrownError);
        }
    });
}

function drawBookingData(bodyData) {
    //setup dates
    var startDateObject = new Date(startDate);
    var oneDay = 24 * 60 * 60 * 1000;
 
    //create dataArray
    var dataArray = [];
    for (var x = 0 ; x < items; x++) {
        dataArray.push([]);
    }

    //fill array with data from the ajax request
    console.log("testie");
    console.log(bodyData);
    console.log(dataArray);
    console.log("endie");
    bodyData.forEach(function (data) {
        var dataBeginIndex = new Date(data.startDate);
        var dataEndIndex = new Date(data.endDate);
        var startIndex = dateDiff(startDateObject, dataBeginIndex, oneDay);
        var lastIndex = startIndex + dateDiff(dataBeginIndex, dataEndIndex, oneDay);
        var roomIndex = hotelIds.indexOf(data.roomID);
        var stdStatus = data.message;
        console.log(startIndex);
        console.log(roomIndex);

        for (var index = startIndex; index < items; index++) {
            var extraStatus;
            extraStatus = index == startIndex ? " start" : "";
            extraStatus = index == lastIndex ? " end" : "";
            dataArray[index][roomIndex] = stdStatus + extraStatus;
        }
    });

    //show actual data
    var date = new Date();
    for (var x = 0; x < items; x++) {
        date.setDate(startDateObject.getDate() + x);
        var dateString = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();

        $("#booking-table tbody").append("<tr data-date=\"" +  dateString  + "\"></tr>");

    }
    $("#booking-table tbody tr").each(function (index) {
        date.setDate(startDateObject.getDate() + index);
        var totalString;
        totalString += "<td>" + date.toDateString() + "</td>";
        for (var x = 0; x < hotelIds.length; x++) {
            var classData = dataArray[index][x];
            var className = classData == undefined ? "free" : classData;
            totalString += "<td class=\"" + className + "\"></td>";
        }
        $(this).append(totalString);
    });

    console.log(dataArray);
}

function checkAlert(data) {
    if (data == "" || data == null) {
        return;
    } else {
        alert(data);
        throw "Sorry, you have to check your input";
    }
}

function dateDiff(fistDate, secondDate, factor) {
    return Math.round(Math.abs(fistDate.getTime() - secondDate.getTime())/factor);
}