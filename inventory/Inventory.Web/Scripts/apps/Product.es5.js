"use strict";

var Product = (function (Product) {

    //Get Asset Categories
    Product.GetAssetCategories = function (url) {
        $("#AssetType").change(function () {
            $("#AssetCategory").empty();
            $('#AssetCategory').append($('<option></option>').val(null).html("Please Select"));

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
            } else {
                $("#AssetCategory").empty();
                $('#AssetCategory').append($('<option></option>').val(null).html("Please Select"));
            }
        });
    };

    return Product;
})({});

