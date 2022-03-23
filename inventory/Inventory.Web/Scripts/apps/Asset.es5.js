"use strict";

var Asset = (function (Asset) {

    //Get Buildings
    Asset.GetBuildings = function (url) {

        $("#Location").change(function () {
            $("#LocationId").val($("#Location").val());
            $("#BuildingId").val(null);
            $("#RoomId").val(null);
            Buildings();
        });

        function Buildings() {
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
                    $("#Room").empty();
                    $('#Room').append($('<option></option>').val(null).html("Please Select"));
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
                $("#Room").empty();
                $('#Room').append($('<option></option>').val(null).html("Please Select"));
            }
        };
    };

    //Get Rooms
    Asset.GetRooms = function (url) {
        $("#Building").change(function () {
            $("#BuildingId").val($("#Building").val());
            $("#RoomId").val(null);

            if ($("#Building").val() > 0) {
                var BuildingOptions = {};
                BuildingOptions.url = url + "?buildingId=" + $("#Building").val();
                BuildingOptions.type = "POST";

                BuildingOptions.data = JSON.stringify({
                    Building: $("#Building").val()
                });

                BuildingOptions.datatype = "json";
                BuildingOptions.contentType = "application/json";

                BuildingOptions.success = function (list) {
                    $("#Room").empty();
                    $('#Room').append($('<option></option>').val(null).html("Please Select"));

                    for (var i = 0; i < list.length; i++) {
                        $('#Room').append($('<option></option>').val(list[i].Id).html(list[i].DisplayName));
                    }
                };
                BuildingOptions.error = function () {
                    console.log("Error in Getting Rooms!!");
                };
                $.ajax(BuildingOptions);
            } else {
                $("#Room").empty();
                $('#Room').append($('<option></option>').val(null).html("Please Select"));
            }
        });
    };

    Asset.UpdateRoomId = function () {
        $("#Room").change(function () {
            $("#RoomId").val($("#Room").val());
        });
    };

    Asset.UpdateAssignedUserId = function (url) {
        $("#AssignedUser").change(function () {

            $("#AssignedUserId").val($("#AssignedUser").val());

            // Get default Location for User
            if ($("#ConnectedAsset").val() == null || $("#ConnectedAsset").val().length === 0) {

                var userId = $("#AssignedUserId").val();

                var userLocationOptions = {};
                userLocationOptions.url = url + "?userId=" + userId;
                userLocationOptions.type = "GET";
                userLocationOptions.datatype = "json";
                userLocationOptions.contentType = "application/json";

                userLocationOptions.success = function (userLocation) {

                    if (userLocation.Id > 0) {
                        $("#Location").val(userLocation.Id).change();
                        $("#LocationId").val(userLocation.Id);
                        $("#Building").val(null).change();
                        $("#BuildingId").val(null);
                        $("#Room").val(null);
                        $("#RoomId").val(null);
                    } else {
                        $("#Location").val(null).change();
                        $("#LocationId").val(null);
                        $("#Building").val(null).change();
                        $("#BuildingId").val(null);
                        $("#Room").val(null);
                        $("#RoomId").val(null);
                        console.log("No Location Found for User!!");
                    }
                };
                userLocationOptions.error = function () {
                    console.log("Error Getting User Location!!");
                };

                $.ajax(userLocationOptions);
            }
        });
    };

    //Hide Show Assigned User / Asset / Connected Asset based on Asset Status
    Asset.Status = function () {
        $(document).ready(function () {
            updateAssignedConnected();
        });

        $("#AssetStatus").change(function () {

            $("#StatusId").val($("#AssetStatus").val());
            $("#Location").prop("disabled", false);
            $("#Building").prop("disabled", false);
            $("#Room").prop("disabled", false);

            updateAssignedConnected();
        });

        function updateAssignedConnected() {
            if ($("#AssetStatus option:selected").text() == 'Available' || $("#AssetStatus option:selected").text() == 'Assigned' || $("#AssetStatus option:selected").text() == 'Shared') {

                if ($("#AssetTypeName").val() && $("#AssetTypeName").val() == "Software") {

                    if ($("#LicenseType").val() && $("#LicenseType").val() == "User" && $("#AssetStatus option:selected").text() == 'Assigned') {

                        $('#AssignedAsset').val(null);
                        $('#AssignedAssetDiv').hide();
                        $("#AssignedAsset").prop("disabled", true);
                        $('#AssignedAsset').selectpicker('refresh');

                        $('#AssignedUserDiv').show();
                        $("#AssignedUser").prop("disabled", false);
                        $('#AssignedUser').selectpicker('refresh');
                    } else if ($("#LicenseType").val() && $("#LicenseType").val() != "User" && $("#AssetStatus option:selected").text() == 'Shared') {

                        $("#AssignedUserId").val(null);
                        $('#AssignedUser').val(null);
                        $('#AssignedUserDiv').hide();
                        $("#AssignedUser").prop("disabled", true);
                        $('#AssignedUser').selectpicker('refresh');

                        $('#AssignedAssetDiv').show();
                        $("#AssignedAsset").prop("disabled", false);
                        $('#AssignedAsset').selectpicker('refresh');
                    } else {

                        $("#AssignedUserId").val(null);
                        $('#AssignedUser').val(null);
                        $('#AssignedUserDiv').hide();
                        $("#AssignedUser").prop("disabled", true);
                        $('#AssignedUser').selectpicker('refresh');

                        $('#AssignedAsset').val(null);
                        $('#AssignedAssetDiv').hide();
                        $("#AssignedAsset").prop("disabled", true);
                        $('#AssignedAsset').selectpicker('refresh');
                    }
                } else if ($("#AssetTypeName").val() && $("#AssetTypeName").val() == "Hardware") {

                    $('#Location').attr("required", true);

                    $("#SurplusDate").attr("required", false);
                    $("#SNFnumber").attr("required", false);
                    $("#SurplusDate").val(null);
                    $('#SNFnumber').val(null);
                    $('#SurplusDateDiv').hide();
                    $('#SNFnumberDiv').hide();

                    if ($("#AssetStatus option:selected").text() == 'Assigned') {

                        $('#AssignedUserDiv').show();
                        $("#AssignedUser").prop("disabled", false);
                        $('#AssignedUser').selectpicker('refresh');
                    } else if ($("#AssetStatus option:selected").text() == 'Available' || $("#AssetStatus option:selected").text() == 'Shared') {

                        $("#AssignedUserId").val(null);
                        $('#AssignedUser').val(null);
                        $('#AssignedUserDiv').hide();
                        $("#AssignedUser").prop("disabled", true);
                        $('#AssignedUser').selectpicker('refresh');
                    }

                    if ($("#Category").val() != "Desktop" && $("#Category").val() != "Laptop" && $("#Category").val() != "Server") {

                        $('#ConnectedAssetDiv').show();
                        $("#ConnectedAsset").prop("disabled", false);
                        $('#ConnectedAsset').selectpicker('refresh');
                    } else {

                        $('#ConnectedAsset').val(null);
                        $('#ConnectedAssetDiv').hide();
                        $("#ConnectedAsset").prop("disabled", true);
                        $('#ConnectedAsset').selectpicker('refresh');
                    }

                    if ($("#Category").val() == "Desktop" || $("#Category").val() == "Laptop" || $("#Category").val() == "Server") {

                        $('#DrawerDiv').show();
                        $("#Drawer").prop("disabled", false);
                    } else {

                        $('#Drawer').val(null);
                        $('#DrawerDiv').hide();
                        $("#Drawer").prop("disabled", true);
                    }
                }
            } else if ($("#AssetStatus option:selected").text() == 'Surplus') {

                $("#Location").attr("required", false);

                $("#AssignedUserId").val(null);
                $('#AssignedUser').val(null);
                $('#AssignedUserDiv').hide();
                $("#AssignedUser").prop("disabled", true);
                $('#AssignedUser').selectpicker('refresh');

                $('#AssignedAsset').val(null);
                $('#AssignedAssetDiv').hide();
                $("#AssignedAsset").prop("disabled", true);
                $('#AssignedAsset').selectpicker('refresh');

                $('#ConnectedAsset').val(null);
                $('#ConnectedAssetDiv').hide();
                $("#ConnectedAsset").prop("disabled", true);
                $('#ConnectedAsset').selectpicker('refresh');

                $('#Drawer').val(null);
                $('#DrawerDiv').hide();
                $("#Drawer").prop("disabled", true);

                $("#SurplusDate").attr("required", true);
                $("#SNFnumber").attr("required", true);
                $('#SurplusDateDiv').show();
                $('#SNFnumberDiv').show();
            } else {

                $("#Location").attr("required", false);

                $("#AssignedUserId").val(null);
                $('#AssignedUser').val(null);
                $('#AssignedUserDiv').hide();
                $("#AssignedUser").prop("disabled", true);
                $('#AssignedUser').selectpicker('refresh');

                $('#AssignedAsset').val(null);
                $('#AssignedAssetDiv').hide();
                $("#AssignedAsset").prop("disabled", true);
                $('#AssignedAsset').selectpicker('refresh');

                $('#ConnectedAsset').val(null);
                $('#ConnectedAssetDiv').hide();
                $("#ConnectedAsset").prop("disabled", true);
                $('#ConnectedAsset').selectpicker('refresh');

                $('#Drawer').val(null);
                $('#DrawerDiv').hide();
                $("#Drawer").prop("disabled", true);

                $("#SurplusDate").attr("required", false);
                $("#SNFnumber").attr("required", false);
                $("#SurplusDate").val(null);
                $('#SNFnumber').val(null);
                $('#SurplusDateDiv').hide();
                $('#SNFnumberDiv').hide();
            }
        };
    };

    Asset.ConnectedAsset = function (url) {
        $(document).ready(function () {
            if ($("#ConnectedAsset").val()) {

                $("#AssetStatus").prop("disabled", true);
                $("#AssignedUser").prop("disabled", true);
                $('#AssignedUser').selectpicker('refresh');
                $("#Location").prop("disabled", true);
                $("#Building").prop("disabled", true);
                $("#Room").prop("disabled", true);
            }
        });

        $("#ConnectedAsset").change(function () {
            GetAsset();
        });

        function GetAsset() {

            //Update Location, Building, Room and Assigned User on Connected Asset Id
            if ($("#ConnectedAsset").val() > 0) {

                $("#AssetStatus").prop("disabled", true);
                $("#AssignedUser").prop("disabled", true);
                $('#AssignedUser').selectpicker('refresh');
                $("#Location").prop("disabled", true);
                $("#Building").prop("disabled", true);
                $("#Room").prop("disabled", true);

                var ConnectedAssetOptions = {};
                ConnectedAssetOptions.url = url + "?assetId=" + $("#ConnectedAsset").val();
                ConnectedAssetOptions.type = "POST";
                ConnectedAssetOptions.datatype = "json";
                ConnectedAssetOptions.contentType = "application/json";

                ConnectedAssetOptions.success = function (Asset) {

                    $("#AssignedUser").val(null);
                    $("#AssignedUserId").val(null);
                    $('#AssignedUser').selectpicker('refresh');

                    $("#Location").val(null);
                    $("#LocationId").val(null);
                    $("#Location").val(Asset.LocationId);
                    $("#LocationId").val(Asset.LocationId);
                    $("#BuildingId").val(null);
                    $("#Building").val(null);
                    $("#RoomId").val(null);
                    $("#Room").val(null);

                    $("#AssetStatus").val(Asset.StatusId);
                    $("#StatusId").val(Asset.StatusId);

                    if (Asset.Status.Name == "Assigned") {
                        $("#AssignedUser").val(Asset.AssignedUserId);
                        $("#AssignedUserId").val(Asset.AssignedUserId);
                        $('#AssignedUserDiv').show();
                        $("#AssignedUser").attr("required", true);
                    } else {
                        $("#AssignedUser").val(null);
                        $("#AssignedUserId").val(null);
                        $('#AssignedUserDiv').hide();
                        $("#AssignedUser").attr("required", false);
                    }

                    $('#AssignedUser').selectpicker('refresh');

                    if (Asset.LocationId && Asset.BuildingId) {
                        $('#Building').append($('<option></option>').val(Asset.Building.Id).html(Asset.Building.Name));
                        $("#Building").val(Asset.BuildingId);
                        $("#BuildingId").val(Asset.BuildingId);
                    } else {
                        $('#Building').append($('<option></option>').val(null).html("Please Select"));
                    }
                    if (Asset.LocationId && Asset.BuildingId && Asset.RoomId) {
                        $('#Room').append($('<option></option>').val(Asset.Room.Id).html(Asset.Room.Name));
                        $("#Room").val(Asset.RoomId);
                        $("#RoomId").val(Asset.RoomId);
                    } else {
                        $('#Room').append($('<option></option>').val(null).html("Please Select"));
                    }
                };
                ConnectedAssetOptions.error = function () {
                    console.log("Error Getting Connected Asset!!");
                };
                $.ajax(ConnectedAssetOptions);
            } else {

                $("#AssetStatus").prop("disabled", false);

                if ($("#AssetStatus option:selected").text() != 'Assigned') {
                    $("#AssignedUser").attr("required", false);
                }

                //$('#AssetStatus option:selected').prop('selected', false);
                //$("#AssetStatus option").filter(function (index) { return $(this).text() === "Available"; }).prop('selected', true);
                $("#StatusId").val($("#AssetStatus").val());

                $("#AssignedUser").prop("disabled", false);
                $('#AssignedUser').selectpicker('refresh');

                $("#Location").prop("disabled", false);
                $("#Building").prop("disabled", false);
                $("#Room").prop("disabled", false);
            }
        }
    };

    Asset.GetBulkAssets = function (url) {
        //Get Assets
        $("#GetAssets").click(function () {

            $("#AssetBulkForm #DateReceived").val(null);
            $("#AssetBulkForm #Status").val(null);
            $("#AssetBulkForm #SurplusDate").val(null);
            $("#AssetBulkForm #SNFnumber").val(null);
            $("#AssetBulkForm #AssignedUser").val(null);
            $("#AssetBulkForm #AssignedAsset").val(null);
            $("#AssetBulkForm #Location").val(null);
            $("#AssetBulkForm #Building").val(null);
            $("#AssetBulkForm #Room").val(null);
            $("#AssetBulkForm #Notes").val(null);

            $("#AssetBulkForm #SurplusDate").attr("required", false);
            $("#AssetBulkForm #SNFnumber").attr("required", false);
            $("#AssetBulkForm #Location").attr("required", false);
            $("#AssetBulkForm #AssignedAsset").attr("required", false);
            $("#AssetBulkForm #AssignedUser").attr("required", false);

            $("#AssetBulkForm #SurplusDateDiv").hide();
            $("#AssetBulkForm #SNFnumberDiv").hide();

            var ids = [];

            $(".checkSingle").each(function () {
                if (this.checked) {
                    ids.push(this.value);
                }
            });

            if (ids.length > 0) {

                $.ajax({
                    type: "GET",
                    url: url + "?assets=" + ids,
                    success: function success(data) {
                        var obj = JSON.parse(data);

                        //console.log(obj);

                        $("#AssetBulkForm #AssetIds").val(obj.AssetIds);
                        $("#AssetBulkForm #DateReceived").val(null);
                        $("#AssetBulkForm #Status").empty();
                        $("#AssetBulkForm #Status").append($('<option></option>').val(null).html("Please Select"));
                        $("#AssetBulkForm #SurplusDate").val(null);
                        $("#AssetBulkForm #SNFnumber").val(null);
                        $("#AssetBulkForm #AssignedUser").empty();
                        $("#AssetBulkForm #AssignedUser").append($('<option></option>').val(null).html("Please Select"));
                        $("#AssetBulkForm #AssignedAsset").empty();
                        $("#AssetBulkForm #AssignedAsset").append($('<option></option>').val(null).html("Please Select"));
                        $("#AssetBulkForm #Location").empty();
                        $("#AssetBulkForm #Location").append($('<option></option>').val(null).html("Please Select"));
                        $("#AssetBulkForm #Building").empty();
                        $("#AssetBulkForm #Building").append($('<option></option>').val(null).html("Please Select"));
                        $("#AssetBulkForm #Room").empty();
                        $("#AssetBulkForm #Room").append($('<option></option>').val(null).html("Please Select"));
                        $("#AssetBulkForm #Notes").val(null);

                        var Statuses = obj.Statuses;
                        var Users = obj.Users;
                        var Assets = obj.ComputerTypeAssets;
                        var Locations = obj.Locations;

                        for (var i = 0; i < Statuses.length; i++) {
                            $("#AssetBulkForm #Status").append($("<option></option>").val(Statuses[i].Id).html(Statuses[i].Name));
                        }

                        for (var i = 0; i < Users.length; i++) {
                            $("#AssetBulkForm #AssignedUser").append($("<option></option>").val(Users[i].UserId).html(Users[i].Name));
                        }

                        for (var i = 0; i < Assets.length; i++) {
                            $("#AssetBulkForm #AssignedAsset").append($("<option></option>").val(Assets[i].Id).html(Assets[i].SerialandAssignedUser));
                        }

                        for (var i = 0; i < Locations.length; i++) {
                            $("#AssetBulkForm #Location").append($("<option></option>").val(Locations[i].Id).html(Locations[i].DisplayName));
                        }

                        $("#AssetBulkForm #Building").val(null);
                        $("#AssetBulkForm #Room").val(null);
                        $('#AssetBulkForm #AssignedUser').selectpicker('refresh');
                        $('#AssetBulkForm #AssignedAsset').selectpicker('refresh');
                        $("#AssetBulkForm #Serialized").val($("#AssetBulkForm").serialize());
                        $("#AssetBulkModalLabel").html($("#GetAssets").data('type'));
                        $("#AssetBulkModalCount").html("&nbsp Total Items: " + obj.AssetCount);
                        $("#AssetBulkModal").modal({});
                    },
                    error: function error() {}
                });
            }
        });

        $("#AssetBulkForm #AssignedUser").change(function () {
            $("#AssetBulkForm #AssignedUserId").val($("#AssignedUser").val());
        });

        $("#AssetBulkForm :input").change(function () {
            $("#AssetBulkForm").data("changed", false);
            $("#AssetBulkForm #EditAssets").prop('disabled', true);

            $("#AssetBulkForm :input:not([type=hidden])").each(function () {
                if ($(this, "input, textarea, select").val()) {
                    $("#AssetBulkForm").data("changed", true);
                    $("#AssetBulkForm #EditAssets").prop('disabled', false);
                }
            });
        });

        $("#AssetBulkForm :input").on('keyup', function () {
            $("#AssetBulkForm").data("changed", false);
            $("#AssetBulkForm #EditAssets").prop('disabled', true);

            $("#AssetBulkForm :input:not([type=hidden])").each(function () {
                if ($(this, "input, textarea, select").val()) {
                    $("#AssetBulkForm").data("changed", true);
                    $("#AssetBulkForm #EditAssets").prop('disabled', false);
                }
            });
        });

        $("#AssetBulkForm #Status").change(function () {

            $("#AssetBulkForm #AssignedUserId").val(null);
            $("#AssetBulkForm #AssignedUser").val(null);

            $("#AssetBulkForm #AssignedUser").val(null);
            $('#AssignedUserDiv').hide();
            $("#AssetBulkForm #AssignedUser").prop("disabled", true);
            $("#AssetBulkForm #AssignedUser").selectpicker('refresh');

            $("#AssetBulkForm #AssignedAsset").val(null);
            $('#AssignedAssetDiv').hide();
            $("#AssetBulkForm #AssignedAsset").prop("disabled", true);
            $("#AssetBulkForm #AssignedAsset").selectpicker('refresh');

            $("#AssetBulkForm #SurplusDate").attr("required", false);
            $("#AssetBulkForm #SNFnumber").attr("required", false);
            $("#AssetBulkForm #SurplusDate").val(null);
            $("#AssetBulkForm #SNFnumber").val(null);
            $("#AssetBulkForm #SurplusDateDiv").hide();
            $("#AssetBulkForm #SNFnumberDiv").hide();

            if ($("#AssetBulkForm #Status option:selected").val() > 0) {

                if ($("#AssetBulkForm #Status option:selected").text() == "Available") {

                    $("#AssetBulkForm #AssignedUserId").val(null);
                    $("#AssetBulkForm #AssignedUser").val(null);
                    $("#AssetBulkForm #AssignedAsset").val(null);

                    $("#AssetBulkForm #Location").val(null);
                    $("#AssetBulkForm #Building").val(null);
                    $("#AssetBulkForm #Room").val(null);

                    $('#AssignedUserDiv').hide();
                    $("#AssetBulkForm #AssignedUser").prop("disabled", true);
                    $("#AssetBulkForm #AssignedUser").selectpicker('refresh');

                    $('#AssignedAssetDiv').hide();
                    $("#AssetBulkForm #AssignedAsset").prop("disabled", true);
                    $("#AssetBulkForm #AssignedAsset").selectpicker('refresh');

                    $("#AssetBulkForm #AssignedAsset").attr("required", false);
                    $("#AssetBulkForm #AssignedUser").attr("required", false);
                } else if ($("#AssetBulkForm #Status option:selected").text() == "Assigned") {

                    $('#AssignedUserDiv').show();
                    $("#AssetBulkForm #AssignedUser").prop("disabled", false);
                    $("#AssetBulkForm #AssignedUser").selectpicker('refresh');

                    ///

                    $("#AssetBulkForm #AssignedAsset").val(null);
                    $("#AssetBulkForm #Location").val(null);
                    $("#AssetBulkForm #Building").val(null);
                    $("#AssetBulkForm #Room").val(null);

                    $('#AssignedAssetDiv').hide();
                    $("#AssetBulkForm #AssignedAsset").prop("disabled", true);
                    $("#AssetBulkForm #AssignedAsset").selectpicker('refresh');

                    $("#AssetBulkForm #AssignedUser").attr("required", true);
                    $("#AssetBulkForm #AssignedAsset").attr("required", false);
                } else if ($("#AssetBulkForm #Status option:selected").text() == "Shared") {

                    $('#AssignedAssetDiv').show();
                    $("#AssetBulkForm #AssignedAsset").prop("disabled", false);
                    $("#AssetBulkForm #AssignedAsset").selectpicker('refresh');

                    ///

                    $("#AssetBulkForm #AssignedUserId").val(null);
                    $("#AssetBulkForm #AssignedUser").val(null);
                    $("#AssetBulkForm #Location").val(null);
                    $("#AssetBulkForm #Building").val(null);
                    $("#AssetBulkForm #Room").val(null);

                    $('#AssignedUserDiv').hide();
                    $("#AssetBulkForm #AssignedUser").prop("disabled", true);
                    $("#AssetBulkForm #AssignedUser").selectpicker('refresh');

                    $("#AssetBulkForm #AssignedAsset").attr("required", true);
                    $("#AssetBulkForm #AssignedUser").attr("required", false);
                } else if ($("#AssetBulkForm #Status option:selected").text() == "Surplus") {

                    $("#AssetBulkForm #SurplusDate").attr("required", true);
                    $("#AssetBulkForm #SNFnumber").attr("required", true);
                    $("#AssetBulkForm #SurplusDateDiv").show();
                    $("#AssetBulkForm #SNFnumberDiv").show();
                } else {

                    $("#AssetBulkForm #AssignedUserId").val(null);
                    $("#AssetBulkForm #AssignedUser").val(null);
                    $("#AssetBulkForm #AssignedAsset").val(null);
                    $("#AssetBulkForm #Location").val(null);
                    $("#AssetBulkForm #Building").val(null);
                    $("#AssetBulkForm #Room").val(null);

                    $('#AssignedUserDiv').hide();
                    $("#AssetBulkForm #AssignedUser").prop("disabled", true);
                    $("#AssetBulkForm #AssignedUser").selectpicker('refresh');

                    $('#AssignedAssetDiv').hide();
                    $("#AssetBulkForm #AssignedAsset").prop("disabled", true);
                    $("#AssetBulkForm #AssignedAsset").selectpicker('refresh');

                    $("#AssetBulkForm #AssignedAsset").attr("required", false);
                    $("#AssetBulkForm #AssignedUser").attr("required", false);

                    $("#AssetBulkForm #SurplusDate").attr("required", false);
                    $("#AssetBulkForm #SNFnumber").attr("required", false);
                    $("#AssetBulkForm #SurplusDate").val(null);
                    $("#AssetBulkForm #SNFnumber").val(null);
                    $("#AssetBulkForm #SurplusDateDiv").hide();
                    $("#AssetBulkForm #SNFnumberDiv").hide();
                }
            } else {

                $('#AssignedUserDiv').show();
                $("#AssetBulkForm #AssignedUser").prop("disabled", false);
                $("#AssetBulkForm #AssignedUser").selectpicker('refresh');

                $('#AssignedAssetDiv').show();
                $("#AssetBulkForm #AssignedAsset").prop("disabled", false);
                $("#AssetBulkForm #AssignedAsset").selectpicker('refresh');

                $("#AssetBulkForm #AssignedAsset").attr("required", false);
                $("#AssetBulkForm #AssignedUser").attr("required", false);

                $("#AssetBulkForm #SurplusDate").attr("required", false);
                $("#AssetBulkForm #SNFnumber").attr("required", false);
                $("#AssetBulkForm #SurplusDate").val(null);
                $("#AssetBulkForm #SNFnumber").val(null);
                $("#AssetBulkForm #SurplusDateDiv").hide();
                $("#AssetBulkForm #SNFnumberDiv").hide();
            }
        });
    };

    Asset.EditBulkAssets = function (url) {

        $("#btn-submit").click(function () {

            var data = $("#AssetBulkForm").serialize();
            var dirty = false;

            if ($("#AssetBulkForm").data("changed")) {
                dirty = true;
            }

            //console.log(data);

            if (dirty) {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: data,
                    success: function success(result) {
                        //console.log(result);
                        if (result.IsUpdated) {
                            $("#AssetBulkModal").modal("hide");
                            window.location.reload();
                        } else {
                            $("#AssetBulkModal").modal("hide");
                            window.location.reload();
                        }
                    }
                });
            }
        });
    };

    return Asset;
})({});

