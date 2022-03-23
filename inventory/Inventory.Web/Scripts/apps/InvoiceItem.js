let InvoiceItem = ((InvoiceItem) => {

    //Get Asset Categories
    InvoiceItem.GetAssetCategories = (url) => {
        $("#AssetType").change(function () {
            $("#AssetCategory").empty();
            $('#AssetCategory').append($('<option></option>').val(null).html("Please Select"));
            $("#Manufacturer").empty();
            $('#Manufacturer').append($('<option></option>').val(null).html("Please Select"));
            $("#Product").empty();
            $('#Product').append($('<option></option>').val(null).html("Please Select"));

            if ($("#AssetType").val() > 0) {
                var AssetTypeOptions = {};
                AssetTypeOptions.url = url + "?assetTypeId=" + $("#AssetType").val();
                AssetTypeOptions.type = "POST";

                AssetTypeOptions.data = JSON.stringify({
                    AssetType: $("#AssetType").val()
                });

                AssetTypeOptions.datatype = "json";
                AssetTypeOptions.contentType = "application/json";

                AssetTypeOptions.success = function (list) {
                    $("#AssetCategory").empty();
                    $('#AssetCategory').append($('<option></option>').val(null).html("Please Select"));
                    for (var i = 0; i < list.length; i++) {
                        $('#AssetCategory').append($('<option></option>').val(list[i].Id).html(list[i].Name));
                    }
                };
                AssetTypeOptions.error = function () {
                    console.log("Error in Getting Categories!!");
                };
                $.ajax(AssetTypeOptions);
            }
            else {
                $("#AssetCategory").empty();
                $('#AssetCategory').append($('<option></option>').val(null).html("Please Select"));
            }
        });
    }

    //Get Manufacturers
    InvoiceItem.GetManufacturers = (url) => {
        $("#AssetType").change(function () {
            $("#AssetCategory").empty();
            $('#AssetCategory').append($('<option></option>').val(null).html("Please Select"));
            $("#Manufacturer").empty();
            $('#Manufacturer').append($('<option></option>').val(null).html("Please Select"));
            $("#Product").empty();
            $('#Product').append($('<option></option>').val(null).html("Please Select"));

            if ($("#AssetType").val() > 0) {
                GetManufacturers(url);
            }
        });

        $("#AssetCategory").change(function () {
            $("#Manufacturer").empty();
            $('#Manufacturer').append($('<option></option>').val(null).html("Please Select"));
            $("#Product").empty();
            $('#Product').append($('<option></option>').val(null).html("Please Select"));

            if ($("#AssetCategory").val() > 0) {
                GetManufacturers(url);
            }
        });

        function GetManufacturers(url) {
            var AssetTypeOptions = {};
            AssetTypeOptions.url = url + "?assetTypeId=" + $("#AssetType").val() + "&assetCategoryId=" + $("#AssetCategory").val();
            AssetTypeOptions.type = "POST";

            AssetTypeOptions.data = JSON.stringify({
                AssetType: $("#AssetType").val()
            });

            AssetTypeOptions.datatype = "json";
            AssetTypeOptions.contentType = "application/json";

            AssetTypeOptions.success = function (list) {
                for (var i = 0; i < list.length; i++) {
                    $('#Manufacturer').append($('<option></option>').val(list[i].Id).html(list[i].DisplayName));
                }
            };
            AssetTypeOptions.error = function () {
                console.log("Error in Getting Manufacturers!!");
            };
            $.ajax(AssetTypeOptions);
        }
    }

    //Get Products
    InvoiceItem.GetProducts = (url) => {
        $("#Manufacturer").change(function () {
            if ($("#Manufacturer").val() > 0) {
                GetProducts(url);
            }
            else {
                $("#Product").empty();
                $('#Product').append($('<option></option>').val(null).html("Please Select"));
            }
        });

        function GetProducts(url) {
            var ManufacturerOptions = {};
            ManufacturerOptions.url = url + "?manuId=" + $("#Manufacturer").val() + "&assetTypeId=" + $("#AssetType").val() + "&assetCategoryId=" + $("#AssetCategory").val();
            ManufacturerOptions.type = "POST";

            ManufacturerOptions.data = JSON.stringify({
                Manufacturer: $("#Manufacturer").val()
            });

            ManufacturerOptions.datatype = "json";
            ManufacturerOptions.contentType = "application/json";

            ManufacturerOptions.success = function (list) {
                $("#Product").empty();
                $('#Product').append($('<option></option>').val(null).html("Please Select"));

                for (var i = 0; i < list.length; i++) {
                    $('#Product').append($('<option></option>').val(list[i].Id).html(list[i].DisplayName));
                }
            };
            ManufacturerOptions.error = function () {
                console.log("Error in Getting Products!!");
            };
            $.ajax(ManufacturerOptions);
        }
    }

    //Get Buildings
    InvoiceItem.GetBuildings = (url) => {
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
            }
            else {
                $("#Building").empty();
                $('#Building').append($('<option></option>').val(null).html("Please Select"));
                $("#Room").empty();
                $('#Room').append($('<option></option>').val(null).html("Please Select"));
            }
        });
    }

    //Get Rooms
    InvoiceItem.GetRooms = (url) => {
        $("#Building").change(function () {
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

                    //$("#Room").prop("disabled", false);
                };
                BuildingOptions.error = function () {
                    console.log("Error in Getting Rooms!!");
                };
                $.ajax(BuildingOptions);
            }
            else {
                $("#Room").empty();
                $('#Room').append($('<option></option>').val(null).html("Please Select"));
            }
        });
    };

    //Hide Show License Types for Invoice Items
    InvoiceItem.InvoiceItemsLicenseTypes = () => {
        $(document).ready(function () {
            showLicenseTypes();
        });

        $("#LicenseType").change(function () {
            showLicenseTypes();
        });

        function showLicenseTypes() {

            if ($("#LicenseType option:selected").val() != "") {
                if ($("#LicenseType option:selected").text() == 'Hardware-Single') {
                    $('#DefaultValuesDiv').show();
                    $('#DateReceivedDiv').show();
                    $("#DateReceived").prop("disabled", false);
                    $('#User').val(null);
                    $('#UserDiv').hide();
                    $("#User").prop("disabled", true);
                    $('#User').selectpicker('refresh');
                    $('#AssetDiv').show();
                    $("#Asset").prop("disabled", false);
                    $('#Asset').selectpicker('refresh');
                    $('#LicenseSingleDiv').show();
                    $("#LicenseSingle").prop("disabled", false);
                    $('#LicenseMulti').val(null);
                    $('#LicenseMultiDiv').hide();
                    $("#LicenseMulti").prop("disabled", true);
                    $('#ExpirationDateDiv').hide();
                    $("#ExpirationDate").prop("disabled", true);
                    $('#ExpirationDate').val(null);
                    $('#ExpirationDateLabel').text("Expiration Date");
                }
                else if ($("#LicenseType option:selected").text() == 'Hardware-Multi') {
                    $('#DefaultValuesDiv').show();
                    $('#DateReceivedDiv').show();
                    $("#DateReceived").prop("disabled", false);
                    $('#User').val(null);
                    $('#UserDiv').hide();
                    $("#User").prop("disabled", true);
                    $('#User').selectpicker('refresh');
                    $('#AssetDiv').show();
                    $("#Asset").prop("disabled", false);
                    $('#Asset').selectpicker('refresh');
                    $('#LicenseSingle').val(null);
                    $('#LicenseSingleDiv').hide();
                    $("#LicenseSingle").prop("disabled", true);
                    $('#LicenseMultiDiv').show();
                    $("#LicenseMulti").prop("disabled", false);
                    $('#ExpirationDateDiv').hide();
                    $("#ExpirationDate").prop("disabled", true);
                    $('#ExpirationDateLabel').text("Expiration Date");
                }
                else if ($("#LicenseType option:selected").text() == 'User') {
                    $('#DefaultValuesDiv').show();
                    $('#DateReceivedDiv').show();
                    $("#DateReceived").prop("disabled", false);
                    $('#UserDiv').show();
                    $("#User").prop("disabled", false);
                    $('#User').selectpicker('refresh');
                    $('#Asset').val(null);
                    $('#AssetDiv').hide();
                    $("#Asset").prop("disabled", true);
                    $('#Asset').selectpicker('refresh');
                    $('#LicenseSingle').val(null);
                    $('#LicenseSingleDiv').hide();
                    $("#LicenseSingle").prop("disabled", true);
                    $('#LicenseMulti').val(null);
                    $('#LicenseMultiDiv').hide();
                    $("#LicenseMulti").prop("disabled", true);
                    $('#ExpirationDateDiv').show();
                    $("#ExpirationDate").prop("disabled", false);
                    $('#ExpirationDateLabel').text("Expiration Date");
                }
            }
            else if ($("#AssetType option:selected").text() == 'Software' || $("#AssetType").val() == 'Software') {
                $('#DefaultValuesDiv').hide();
                $('#DateReceivedDiv').hide();
                $("#DateReceived").prop("disabled", true);
                $('#User').val(null);
                $('#UserDiv').hide();
                $("#User").prop("disabled", true);
                $('#User').selectpicker('refresh');
                $('#Asset').val(null);
                $('#AssetDiv').hide();
                $("#Asset").prop("disabled", true);
                $('#Asset').selectpicker('refresh');
                $('#LicenseSingle').val(null);
                $('#LicenseSingleDiv').hide();
                $("#LicenseSingle").prop("disabled", true);
                $('#LicenseMulti').val(null);
                $('#LicenseMultiDiv').hide();
                $("#LicenseMulti").prop("disabled", true);
                $('#ExpirationDateDiv').hide();
                $("#ExpirationDate").prop("disabled", true);
                $('#ExpirationDateLabel').text("Expiration Date");
            }

        };
    }

    //Hide Show Invoice Items
    InvoiceItem.InvoiceItemsHardwareSoftware = () => {
        $(document).ready(function () {

            if ($("#Location").val() == "") {

                $("#Building").empty();
                $('#Building').append($('<option></option>').val(null).html("Please Select"));
                $("#Room").empty();
                $('#Room').append($('<option></option>').val(null).html("Please Select"));
            }

            if ($("#Building").val() == "") {

                $("#Room").empty();
                $('#Room').append($('<option></option>').val(null).html("Please Select"));
            }

            showHardwareSoftware();
        });

        $("#AssetType").change(function () {
            $('#User').val(null);
            showHardwareSoftware();
        });

        function showHardwareSoftware() {

            if ($("#AssetType option:selected").val() != "") {
                if ($("#AssetType option:selected").text() == 'Hardware') {
                    $('#AssetCategoryDiv').show();
                    $("#AssetCategory").prop("disabled", false);
                    $('#ManuDiv').show();
                    $("#Manufacturer").prop("disabled", false);
                    $('#ManuLabel').text("Make");
                    $('#ProductDiv').show();
                    $("#Product").prop("disabled", false);
                    $('#ProductLabel').text("Model");
                    $('#LicenseType').val(null);
                    $('#LicenseTypeDiv').hide();
                    $("#LicenseType").prop("disabled", true);
                    $("#LicenseType").removeAttr('required');

                    $('#QuantityDiv').show();
                    $("#Quantity").prop("disabled", false);

                    $('#LicenseSingle').val(null);
                    $('#LicenseSingleDiv').hide();
                    $("#LicenseSingle").prop("disabled", true);
                    $('#LicenseMulti').val(null);
                    $('#LicenseMultiDiv').hide();
                    $("#LicenseMulti").prop("disabled", true);
                    $('#SerialDiv').show();
                    $("#Serial").prop("disabled", false);
                    $('#ExpirationDateDiv').show();
                    $("#ExpirationDate").prop("disabled", false);
                    $('#ExpirationDateLabel').text("End of Warranty");

                    $('#DefaultValuesDiv').show();
                    $('#DateReceivedDiv').show();
                    $("#DateReceived").prop("disabled", false);
                    $('#LocationDiv').show();
                    $("#Location").prop("disabled", false);
                    $('#BuildingDiv').show();
                    $("#Building").prop("disabled", false);
                    $('#RoomDiv').show();
                    $("#Room").prop("disabled", false);
                    $('#Asset').val(null);
                    $('#AssetDiv').hide();
                    $("#Asset").prop("disabled", true);
                    $('#Asset').selectpicker('refresh');
                    $('#UserDiv').show();
                    $("#User").prop("disabled", false);
                    $('#User').selectpicker('refresh');

                }
                else if ($("#AssetType option:selected").text() == 'Software') {
                    $('#AssetCategoryDiv').show();
                    $("#AssetCategory").prop("disabled", false);
                    $('#ManuDiv').show();
                    $("#Manufacturer").prop("disabled", false);
                    $('#ManuLabel').text("Manufacturer");
                    $('#ProductDiv').show();
                    $("#Product").prop("disabled", false);
                    $('#ProductLabel').text("Product");
                    $('#LicenseTypeDiv').show();
                    $("#LicenseType").prop("disabled", false);
                    $('#LicenseType').attr("required", true);

                    $('#QuantityDiv').show();
                    $("#Quantity").prop("disabled", false);

                    $('#Serial').val(null);
                    $('#SerialDiv').hide();
                    $('#Serial').prop("disabled", true);
                    $('#ExpirationDateDiv').hide();
                    $("#ExpirationDate").prop("disabled", true);
                    $('#ExpirationDateLabel').text("Expiration Date");


                    $('#DefaultValuesDiv').hide();
                    $('#DateReceivedDiv').hide();
                    $("#DateReceived").prop("disabled", true);
                    $('#Location').val(null);
                    $('#LocationDiv').hide();
                    $('#Location').prop("disabled", true);
                    $('#Building').val(null);
                    $('#BuildingDiv').hide();
                    $('#Building').prop("disabled", true);
                    $('#Room').val(null);
                    $('#RoomDiv').hide();
                    $('#Room').prop("disabled", true);
                    $('#AssetDiv').hide();
                    $('#Asset').prop("disabled", true);
                    $('#User').selectpicker('refresh');
                    $('#UserDiv').hide();
                    $("#User").prop("disabled", true);
                    $('#User').selectpicker('refresh');
                }
            }
            else {

                $('#AssetCategory').val(null);
                $('#AssetCategoryDiv').hide();
                $("#AssetCategory").prop("disabled", true);
                $('#Manufacturer').val(null);
                $('#ManuDiv').hide();
                $("#Manufacturer").prop("disabled", true);
                $('#ManuLabel').text("Make");
                $('#Product').val(null);
                $('#ProductDiv').hide();
                $("#Product").prop("disabled", true);
                $('#ProductLabel').text("Model");
                $('#LicenseSingle').val(null);
                $('#LicenseSingleDiv').hide();
                $("#LicenseSingle").prop("disabled", true);
                $('#LicenseMulti').val(null);
                $('#LicenseMultiDiv').hide();
                $("#LicenseMulti").prop("disabled", true);

                $('#Quantity').val(null);
                $('#QuantityDiv').hide();
                $("#Quantity").prop("disabled", true);

                $('#LicenseType').val(null);
                $('#LicenseTypeDiv').hide();
                $("#LicenseType").prop("disabled", true);
                $('#License').val(null);
                $('#LicenseDiv').hide();
                $("#License").prop("disabled", true);
                $('#Serial').val(null);
                $('#SerialDiv').hide();
                $("#Serial").prop("disabled", true);
                $('#ExpirationDateDiv').hide();
                $("#ExpirationDate").prop("disabled", true);
                $('#ExpirationDate').val(null);
                $('#ExpirationDateLabel').text("Expiration Date");

                $('#DefaultValuesDiv').hide();
                $('#DateReceivedDiv').hide();
                $("#DateReceived").prop("disabled", true);
                $('#Location').val(null);
                $('#LocationDiv').hide();
                $("#Location").prop("disabled", true);
                $('#Building').val(null);
                $('#BuildingDiv').hide();
                $("#Building").prop("disabled", true);
                $('#Room').val(null);
                $('#RoomDiv').hide();
                $("#Room").prop("disabled", true);
                $('#Asset').val(null);
                $('#AssetDiv').hide();
                $("#Asset").prop("disabled", true);
                $('#Asset').selectpicker('refresh');
                $('#User').val(null);
                $('#UserDiv').hide();
                $("#User").prop("disabled", true);
                $('#User').selectpicker('refresh');
            }
        };
    }

    //Hide Show Defaults
    InvoiceItem.ShowHideDefaults = (QuantityOriginal) => {

        $(document).ready(function () {
            if (($("#Quantity").val() != null) && ($("#Quantity").val() > QuantityOriginal)) {
                $('#ShowHideDefaultsDiv').show();
                $('#Location').attr("required", true);
                $('#DateReceived').attr("required", true);
            }
            else {
                $('#ShowHideDefaultsDiv').hide();
                $('#DateReceived').removeAttr('required');
                $('#LicenseMulti').val(null);
                $('#Serial').val(null);
                $('#Location').val(null);
                $('#Location').removeAttr('required');
                $('#Building').val(null);
                $('#Room').val(null);
                $('#Asset').val(null);
                $('#User').val(null);
                $('#User').selectpicker('refresh');
                $('#Asset').selectpicker('refresh');

            }
        });


        $("#Quantity").keyup(function () {

            if (($("#Quantity").val() != null) && ($("#Quantity").val() > QuantityOriginal)) {
                $('#ShowHideDefaultsDiv').show();
                $('#DateReceived').attr("required", true);
                $('#Location').attr("required", true);

            }
            else {
                $('#ShowHideDefaultsDiv').hide();
                $('#DateReceived').removeAttr('required');
                $('#LicenseMulti').val(null);
                $('#Serial').val(null);
                $('#Location').val(null);
                $('#Location').removeAttr('required');

                $('#Building').val(null);
                $('#Room').val(null);
                $('#Asset').val(null);
                $('#User').val(null);
                $('#User').selectpicker('refresh');
                $('#Asset').selectpicker('refresh');
            }

        });

        $("#Quantity").change(function () {

            if (($("#Quantity").val() != null) && ($("#Quantity").val() > QuantityOriginal)) {
                $('#ShowHideDefaultsDiv').show();
                $('#DateReceived').attr("required", true);
                $('#Location').attr("required", true);
            }
            else {
                $('#ShowHideDefaultsDiv').hide();
                $('#DateReceived').removeAttr('required');
                $('#LicenseMulti').val(null);
                $('#Serial').val(null);
                $('#Location').val(null);
                $('#Location').removeAttr('required');
                $('#Building').val(null);
                $('#Room').val(null);
                $('#Asset').val(null);
                $('#User').val(null);
                $('#User').selectpicker('refresh');
                $('#Asset').selectpicker('refresh');
            }

        });
    }

    //Get Default Location
    InvoiceItem.UpdateLocation = (url) => {

        $("#User").change(function () {

            var userLocationOptions = {};
            userLocationOptions.url = url + "?userId=" + $("#User").val();
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
                }
                else {
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

        });
    };

    return InvoiceItem;
})({});