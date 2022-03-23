let Invoice = ((Invoice) => {

    //Clear Form
    Invoice.ClearForm = (form) => {
        $("#" + form + " #" + "InvoiceId").val(null);
        $("#" + form + " #" + "PONumberOriginal").val(null);
        $("#" + form + " #" + "PONumber").val(null);
        $("#" + form + " #" + "Supplier").val(null);
        $("#" + form + " #" + "TotalPrice").val(null);
        $("#" + form + " #" + "PurchaseDate").val(null);
    }

    //Clear Validation
    Invoice.ClearValidation = (form) => {
        var myForm = document.getElementById(form);
        var validator = $(myForm).validate();
        $('[name]', myForm).each(function () {
            validator.successList.push(this);
            validator.showErrors();
        });
        validator.resetForm();
        validator.reset();
    }

    //Sort Drop Down
    Invoice.SortDropDown = (selectId) => {
        var foption = $(selectId + ' option:first');
        var soptions = $(selectId + ' option:not(:first)').sort(function (a, b) {
            return a.text == b.text ? 0 : a.text < parseInt(b.text) ? -1 : 1
        });
        $(selectId).html(soptions).prepend(foption);
    }

    //Fill Supplier DropDown
    Invoice.Suppliers = (list) => {
      
        $("#InvoiceForm #Supplier").empty();
        $('#InvoiceForm #Supplier').append($('<option></option>').val(null).html("Please Select"));

        if ($(list).length > 0) {
                for (var i = 0; i < list.length; i++) {
                    $("#InvoiceForm #Supplier").append($('<option></option>').val(list[i].Id).html(list[i].DisplayName));
                }
        }
    }

    //Get Suppliers
    Invoice.GetSuppliers = (url) => {
        var AssetTypeOptions = {};

        AssetTypeOptions.url = url + "Asset/Suppliers";
        AssetTypeOptions.type = "POST";
        AssetTypeOptions.datatype = "json";
        AssetTypeOptions.contentType = "application/json";

        AssetTypeOptions.success = function (list) {
            Invoice.Suppliers(list);
        };
        AssetTypeOptions.error = function () {
            console.log("Error in Getting Suppliers!!");
        };
        $.ajax(AssetTypeOptions);
    }

    //Get Blank Invoice
    Invoice.GetBlankInvoice = (url) => {
        $("#BlankInvoice").click(function () {
            Invoice.ClearForm("InvoiceForm");
            Invoice.ClearValidation("InvoiceForm");

            $.when(Invoice.GetSuppliers(url)).done(function () {
                $("#InvoiceForm #EditInvoice").hide();
                $("#InvoiceForm #CreateInvoice").show();
                $("#InvoiceModalLabel").html($("#BlankInvoice").data('type'));
                $('#InvoiceModal').modal({});
            });

        })
    }

    //Get Invoice
    Invoice.GetInvoice = (url) => {
        $("#GetInvoice").click(function () {
            if ($("#InvoiceItemForm #InvoiceNumber").val()) {
                var options = { year: 'numeric', month: '2-digit', day: '2-digit' };
                Invoice.ClearForm("InvoiceForm");
                Invoice.ClearValidation("InvoiceForm");
                $.ajax({
                    type: "Get",
                    url: url,
                    data: { "Id": $("#InvoiceItemForm #InvoiceNumber").val() },
                    success: function (data) {
                        var obj = JSON.parse(data);
                        $("#InvoiceForm #InvoiceId").val(obj.Id);
                        $("#InvoiceForm #PONumberOriginal").val(obj.PONumber);
                        $("#InvoiceForm #PONumber").val(obj.PONumber);

                        $("#InvoiceForm #SupplierIdOriginal").val(obj.SupplierIdOriginal);
                        $("#InvoiceForm #SupplierDisplayNameOriginal").val(obj.SupplierDisplayNameOriginal);
                        
                        $("#InvoiceForm #TotalPrice").val(obj.TotalPrice ? obj.TotalPrice.toFixed(2) : null);
                        //$("#InvoiceForm #PurchaseDate").val(obj.PurchaseDate ? new Date(obj.PurchaseDate).toLocaleDateString('en-us', options) : "");
                        $("#InvoiceForm #PurchaseDate").val(obj.PurchaseDate ? new Date(obj.PurchaseDate).toISOString().slice(0, 10) : "");

                        $.when(Invoice.Suppliers(obj.Suppliers)).done(function () {
                            $("#InvoiceForm #Supplier").val(obj.SupplierId);

                            $("#InvoiceModalLabel").html($("#GetInvoice").data('type'));
                            $("#InvoiceForm #EditInvoice").show();
                            $("#InvoiceForm #CreateInvoice").hide();
                            $('#InvoiceModal').modal({});
                        });

                    }
                })
            }
        })
    }

    //Create Invoice
    Invoice.CreateInvoice = (url) => {
        $("#CreateInvoice").click(function () {
            var data = $("#InvoiceForm").serialize();
            $.ajax({
                type: "Post",
                url: url,
                data: data,
                success: function (result) {
                    //console.log(result);
                    if (result.IsCreated) {

                        $("#InvoiceItemForm #InvoiceNumber").append($('<option></option>').val(result.Content.Id).text(result.Content.PONumber));
                        Invoice.SortDropDown("#InvoiceItemForm #InvoiceNumber");
                        $("#InvoiceItemForm #InvoiceNumber").val(result.Content.Id);
                        $("#InvoiceItemForm #InvoiceNumber").selectpicker('refresh');

                        $("#InvoiceModal").modal("hide");
                    }
                    else {
                        $.when(Invoice.Suppliers(result.Content.Suppliers)).done(function () {
                            $("#InvoiceForm #Supplier").val(result.Content.SupplierId);

                            $.each(result.errors, function (key, value) {
                                $("#InvoiceForm [id=\"" + "Val_" + key + "\"]").html(value);
                                $("#" + key)[0].setCustomValidity('Error');
                            });

                        });
                        $("#CreateInvoice").attr("disabled", true);
                        $("#EditInvoice").attr("disabled", true);
                    }
                }
            })
        })
    }

    //Save Invoice
    Invoice.EditInvoice = (url) => {
        $("#EditInvoice").click(function () {
            var data = $("#InvoiceForm").serialize();
            $.ajax({
                type: "Post",
                url: url + "/" + $('#InvoiceId').val(),
                data: data,
                success: function (result) {
                    //console.log(result);
                    if (result.IsUpdated) {
                        $("#InvoiceItemForm #InvoiceNumber option:selected").text(result.Content.PONumber);
                        Invoice.SortDropDown('#InvoiceNumber');
                        $("#InvoiceItemForm #InvoiceNumber").val(result.Content.Id);
                        $("#InvoiceItemForm #InvoiceNumber").selectpicker('refresh');
                        $("#InvoiceModal").modal("hide");
                    }
                    else {
                        $.when(Invoice.Suppliers(result.Content.Suppliers)).done(function () {
                            $("#InvoiceForm #Supplier").val(result.Content.SupplierId);

                            $.each(result.errors, function (key, value) {
                                $("#InvoiceForm [id=\"" + "Val_" + key + "\"]").html(value);
                                $("#" + key)[0].setCustomValidity('Error');
                            });
                        });
                        $("#CreateInvoice").attr("disabled", true);
                        $("#EditInvoice").attr("disabled", true);
                    }
                }
            })
        })
    }

    Invoice.DisableEnableSubmit = (formname) => {

        $(":input").on("keyup", function () {
            this.setCustomValidity('');
            if ($(formname)[0].checkValidity()) {
                $("#CreateInvoice").removeAttr("disabled");
                $("#EditInvoice").removeAttr("disabled");
            }
            else {
                $("#CreateInvoice").attr("disabled", true);
                $("#EditInvoice").attr("disabled", true);
            }
        });

        $(":input").on("blur", function () {
            this.setCustomValidity('');
            if ($(formname)[0].checkValidity()) {
                $("#CreateInvoice").removeAttr("disabled");
                $("#EditInvoice").removeAttr("disabled");
            }
            else {
                $("#CreateInvoice").attr("disabled", true);
                $("#EditInvoice").attr("disabled", true);
            }
        });

        $(":input").change(function () {
            this.setCustomValidity('');
            if ($(formname)[0].checkValidity()) {
                $("#CreateInvoice").removeAttr("disabled");
                $("#EditInvoice").removeAttr("disabled");
            }
            else {
                $("#CreateInvoice").attr("disabled", true);
                $("#EditInvoice").attr("disabled", true);
            }
        });
    }

    return Invoice;
})({});