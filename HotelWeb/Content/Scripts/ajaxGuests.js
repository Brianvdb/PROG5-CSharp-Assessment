/// <reference path="../Scripts/jquery-2.1.1.js" />
var maxPersonen;
var curIndex = 0;

$(document).ready(function () {
    maxPersonen = $("#customer-container").data("maxpersons");
    console.log("Maximaal aantal personen: " + maxPersonen);
    addPerson();

    $("#add-person-button").click(function () {
        addPerson();
    });

    $("#customer-container").on("click", ".remove-person-button", function () {
        removePerson($(this));
    });

    $("#guest-form").submit(function () {
        var allCorrect = true;
        guestObject = [];
        $("#customer-container fieldset").each(function (index) {
            var birthdate = $(".birthdate-input", this).val();
            var firstName = $(".first-name-input", this).val();
            var lastName = $(".last-name-input", this).val();

            if (firstName == "" || lastName == "") {
                alert("Controleer of alle gegevens zijn ingevuld. Dat is nu NIET het geval.");
                allCorrect = false;
                return;
            }
            guestObject[index] = {};
            guestObject[index].firstName = firstName;
            guestObject[index].lastName = lastName;
            guestObject[index].birthDate = birthdate;
            guestObject[index].sex = $('input[type=radio]:checked', this).val();
        });
        console.log(guestObject);
        console.log(JSON.stringify(guestObject));
        if (allCorrect === true) {
            $.ajax({
                type: "POST",
                url: '/Book/RegisterAdress',
                dataType: "json",
                traditional: true,
                contentType: 'application/json',
                data: JSON.stringify(guestObject),
                success: function (data) {
                    console.log(data);
                },
                error: function (data) {
                    console.log(data);
                    alert("failure");
                }
            });  
        } else {
            return false;
        }
        return true;
    });
});

function newPersonForm() {
    var index = curIndex++;
    return "<fieldset>" +
        addLabel("Voornaam:") +
        addInput("text", "first-name-input") +
        addLabel("Achternaam:") +
        addInput("text", "last-name-input") +
        addLabel("Uw geboortedatum:") +
        addInput("date", "birthdate-input") +
        addLabel("Geslacht:") +
        "Man: <input type=\"radio\" name=\"sex" + index + "\" value=\"male\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
        "Vrouw: <input type=\"radio\" name=\"sex" + index + "\" value=\"female\"><br /><br />" +
        "<input type=\"button\" class=\"remove-person-button\" value=\"Persoon Verwijderen\" />"
}

function addPerson() {
    if ($("#customer-container fieldset").length < maxPersonen) {
        $("#customer-container").append(newPersonForm);
    } else {
        alert("Helaas is deze kamer kunnen maximaal " + maxPersonen + " personen.");
    }
}

function removePerson($clickedButton) {
    $clickedButton.parent().remove();
}

function addLabel(labelNaam){
    return "<div class=\"editor-label\"><label for=\"\">" + labelNaam + "</label></div>";
}

function addInput(type, className) {
    return "<div class=\"editor-field\"><input type=\"" + type + "\" class=\"" + className + "\" /></div>"
}