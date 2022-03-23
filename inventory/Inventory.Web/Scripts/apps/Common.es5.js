"use strict";

var Common = (function (Common) {

    //Phone Format
    Common.PhoneFormat = function () {
        $("input[id='Phone']").on("input", function () {
            $("input[id='Phone']").val(destroyMask(this.value));
            this.value = createMask($("input[id='Phone']").val());
        });

        function createMask(string) {
            return string.replace(/(\d{3})(\d{3})(\d{4})/, "$1-$2-$3");
        }

        function destroyMask(string) {
            return string.replace(/\D/g, '').substring(0, 10);
        }
    };

    //Date Picker
    Common.DatePicker = function () {

        $('input[type=datetime]').datepicker({
            dateFormat: "mm/dd/yy",
            changeMonth: true,
            changeYear: true,
            yearRange: "-20:+10"
        });

        var dateControl = document.querySelector('input[type="date"]');
        if (dateControl !== 'undefined' && dateControl != null) {
            var date = new Date();
            var min = 20;
            var max = 10;
            var minDate = new Date(date.setFullYear(date.getFullYear() - min));
            var maxDate = new Date(date.setFullYear(date.getFullYear() + max + min));
            dateControl.min = minDate.getFullYear() + "-" + ("0" + (minDate.getMonth() + 1)).slice(-2) + "-" + ("0" + minDate.getDate()).slice(-2);
            dateControl.max = maxDate.getFullYear() + "-" + ("0" + (maxDate.getMonth() + 1)).slice(-2) + "-" + ("0" + maxDate.getDate()).slice(-2);
        }
    };

    //Disable / Enable Submit
    Common.DisableEnableSubmit = function (formname) {

        $(":input").on("keyup", function () {
            this.setCustomValidity('');
            if ($(formname)[0].checkValidity()) {
                $("#btn-submit").removeAttr("disabled");
            } else {
                $("#btn-submit").attr("disabled", true);
            }
        });

        //$(":input").on("blur", function () {
        //    this.setCustomValidity('');
        //    if ($(formname)[0].checkValidity()) {
        //        $("#btn-submit").removeAttr("disabled");
        //    }
        //    else {
        //        $("#btn-submit").attr("disabled", true);
        //    }
        //});

        $(":input").change(function () {
            this.setCustomValidity('');
            if ($(formname)[0].checkValidity()) {
                $("#btn-submit").removeAttr("disabled");
            } else {
                $("#btn-submit").attr("disabled", true);
            }
        });

        $(document).ready(function () {
            if ($(formname).find(".input-validation-error").length == 0 && $(formname)[0].checkValidity()) {
                $("#btn-submit").removeAttr("disabled");
            } else {
                $("#btn-submit").attr("disabled", true);
                var element = $(formname).find(".input-validation-error");
                if (element.length > 0) {
                    $("#" + element[0].id)[0].setCustomValidity('Error');
                }
            }
        });
    };

    //Remove White Space
    Common.RemoveWhiteSpace = function () {
        $('.removewhitespace').change(function () {
            $(this).val($(this).val().replace(/ /g, ""));
            $(this).val($(this).val().replace(/[\r\n]+/gm, ""));
        });
    };

    Common.CheckedAll = function () {

        $(document).ready(function () {

            $("#checkedAll").change(function () {
                if (this.checked) {
                    $(".checkSingle").each(function () {
                        this.checked = true;
                    });
                } else {
                    $(".checkSingle").each(function () {
                        this.checked = false;
                    });
                }
            });

            $(".checkSingle").click(function () {

                if ($(this).is(":checked")) {

                    var isAllChecked = 0;

                    $(".checkSingle").each(function () {
                        if (!this.checked) {
                            isAllChecked = 1;
                        }
                    });

                    if (isAllChecked == 0) {
                        $("#checkedAll").prop("checked", true);
                    }
                } else {
                    $("#checkedAll").prop("checked", false);
                }
            });
        });
    };

    return Common;
})({});

