let Location = ((Location) => {

    //Alias
    Location.Alias = (url) => {

        $("#Alias").on('beforeItemAdd', function (event) {
            var tag = event.item;
            var $validator = $("form").validate();

            var errors = { LocationAliasNames: "" };
            $validator.showErrors(errors);

            var items = $("#OriginalLocationAliasNames").val().split(",");

            var returnedData = $.grep(items, function (element, index) {
                return element.trim().toLowerCase() == tag.trim().toLowerCase();
            });

            if (returnedData.length == 0) {

                var AliasOptions = {};
                AliasOptions.url = url + "?locId=" + $("#Id").val() + "&alias=" + event.item;
                AliasOptions.type = "GET";
                AliasOptions.datatype = "json";
                AliasOptions.contentType = "application/json";

                AliasOptions.success = function (location) {

                    if (location != "") {
                        errors = { LocationAliasNames: "Alias " + "'" + tag + "'" + " already exist for " + "'" + location + "'" };
                        $validator.showErrors(errors);
                        $('#Alias').tagsinput('remove', tag, { preventPost: true });
                    }
                }

                AliasOptions.error = function () {
                    console.log("error");
                    $('#Alias').tagsinput('remove', tag, { preventPost: true });
                }

                $.ajax(AliasOptions);
            }

        });

        $("#Alias").change(function () {
            $("#LocationAliasNames").val($("#Alias").val().trim());
        });

    }

    return Location;
})({});