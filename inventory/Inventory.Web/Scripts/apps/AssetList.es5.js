"use strict";

var AssetList = (function (AssetList) {

    //AddToList
    AssetList.AddToList = function (assetType) {

        $("#AddToList").click(function () {

            var ids = [];

            $(".checkSingle").each(function () {
                if (this.checked) {
                    ids.push(this.value);
                }
            });

            if (ids.length > 0) {

                $.ajax({
                    type: "GET",
                    url: "AssetList/GetAssetList" + "?assetType=" + assetType,
                    success: function success(data) {

                        var obj = JSON.parse(JSON.stringify(data));
                        $("#AssetListForm #Ids").val(ids);
                        $("#AssetListForm #AssetTypeName").val(assetType);

                        $("#AssetListForm #ListId").empty();
                        $("#AssetListForm #ListId").append($('<option></option>').val(null).html("Please Select"));

                        for (var i = 0; i < obj.length; i++) {
                            $("#AssetListForm #ListId").append($("<option></option>").val(obj[i].Id).html(obj[i].Name));
                        }

                        $("#AssetListModalLabel").html("Add Items To List");
                        $("#AssetListModalCount").html("&nbsp Total Items: " + ids.length);
                        $("#AssetListModal #btn-add").show();
                        $("#AssetListModal #btn-remove").hide();
                        $("#AssetListModal").modal({});
                    },
                    error: function error() {}
                });
            }
        });

        $("#AssetListForm #btn-add").click(function () {

            var data = $("#AssetListForm").serialize();

            var dirty = false;
            var dirty = true;

            if ($("#AssetListForm").data("changed")) {
                dirty = true;
            }

            if (dirty) {
                $.ajax({
                    type: "POST",
                    url: "AssetList/AddAssetsToUserList",
                    data: data,
                    success: function success(result) {

                        if (result.IsUpdated) {
                            $("#AssetList").modal("hide");
                            window.location.reload();
                        } else {
                            $("#AssetList").modal("hide");
                            window.location.reload();
                        }
                    },
                    error: function error(result) {
                        console.log("Error Adding Items to List");
                    }
                });
            }
        });
    };

    //Remove From List
    AssetList.RemoveFromList = function (listId, assetType) {

        $("#RemoveFromList").click(function () {

            var ids = [];

            $(".checkSingle").each(function () {
                if (this.checked) {
                    ids.push(this.value);
                }
            });

            if (ids.length > 0) {

                $.ajax({
                    type: "GET",
                    url: "AssetList/GetAssetListById" + "?assetListId=" + listId + "&assetType=" + assetType,
                    success: function success(data) {

                        var obj = JSON.parse(JSON.stringify(data));

                        $("#AssetListForm #Ids").val(ids);
                        $("#AssetListForm #AssetTypeName").val(assetType);

                        $("#AssetListForm #ListId").empty();

                        for (var i = 0; i < obj.length; i++) {
                            $("#AssetListForm #ListId").append($("<option></option>").val(obj[i].Id).html(obj[i].Name));
                        }

                        $("#AssetListModalLabel").html("Remove Items From List");
                        $("#AssetListModalCount").html("&nbsp Total Items: " + ids.length);
                        $("#AssetListModal #btn-add").hide();
                        $("#AssetListModal #btn-remove").show();
                        $("#AssetListModal").modal({});
                    },
                    error: function error() {}
                });
            }
        });

        $("#AssetListForm #btn-remove").click(function () {

            var data = $("#AssetListForm").serialize();

            var dirty = false;
            var dirty = true;

            if ($("#AssetListForm").data("changed")) {
                dirty = true;
            }

            if (dirty) {
                $.ajax({
                    type: "POST",
                    url: "AssetList/RemoveAssetsFromUserList",
                    data: data,
                    success: function success(result) {

                        if (result.IsUpdated) {
                            $("#AssetList").modal("hide");
                            window.location.reload();
                        } else {
                            $("#AssetList").modal("hide");
                            window.location.reload();
                        }
                    },
                    error: function error(result) {
                        console.log("Error Removing Items From List");
                    }
                });
            }
        });
    };

    return AssetList;
})({});

