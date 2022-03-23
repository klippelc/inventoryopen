"use strict";

var Room = (function (Room) {

    //Get Buildings
    Room.GetBuildings = function (url) {
        $("#Location").change(function () {
            if ($("#Location").val() > 0) {
                var LocationOptions = {};
                LocationOptions.url = url + "?locationId=" + $("#Location").val();
                LocationOptions.type = "POST";

                LocationOptions.data = JSON.stringify({
                    Location: $("#Location").val()
                });

                LocationOptions.datatype = "json";
                LocationOptions.contentType = "application/json";

                LocationOptions.success = function (list) {
                    $("#Building").empty();
                    $('#Building').append($('<option></option>').val(null).html("Please Select"));
                    for (var i = 0; i < list.length; i++) {
                        $('#Building').append($('<option></option>').val(list[i].Id).html(list[i].DisplayName));
                    }

                    //$("#Building").prop("disabled", false);
                };
                LocationOptions.error = function () {
                    console.log("Error in Getting Buildings!!");
                };
                $.ajax(LocationOptions);
            } else {
                $("#Building").empty();
                $('#Building').append($('<option></option>').val(null).html("Please Select"));
            }
        });
    };
    return Room;
})({});

