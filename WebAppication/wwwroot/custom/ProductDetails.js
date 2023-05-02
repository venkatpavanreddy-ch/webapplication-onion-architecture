$(window).load(function () {


    $("#ProductCode").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '../Home/GetProducts',
                dataType: "json",
                data: { searchTerm: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.ItemCode + ", " + item.ItemDescription, value: item.ItemCode }
                    }));
                }
            })
        }
    });


    $("#chk_selectall").click(function () {
        if ($(this).is(':checked')) {
            var id = "all";
            $(".tabs__nav_link").each(function (i, obj) {
                if ($(this).hasClass('is__active')) {
                    id = $(this).attr("id");
                }
            })
            $('.chk_document').each(function (i, obj) {
                if ($(this).hasClass(id)) {
                    $(this).prop('checked', true);
                }
            });
        }
        else {
            $('.chk_document').each(function (i, obj) {
                $(this).prop('checked', false);
            });
        }
    });

    $(".chk_document").click(function () {
        if (!$(this).is(':checked')) {
            $("#chk_selectall").prop('checked', false);
        }
        else {
            var id = "all";
            $(".tabs__nav_link").each(function (i, obj) {
                if ($(this).hasClass('is__active')) {
                    id = $(this).attr("id");
                }
            })
            var isAllchecked = true;
            $('.chk_document').each(function () {
                if (!$(this).is(':checked') && $(this).hasClass(id)) {
                    $("#chk_selectall").prop('checked', false);
                    isAllchecked = false;
                }
            });
            if (isAllchecked)
                $("#chk_selectall").prop('checked', true);
        }
    });

    $("#btnEmailSubmit").click(function () {
        $('#overlay').show();
        clearmessages();
        var documents = [];
        $('.chk_document').each(function (i, obj) {
            if ($(this).is(':checked')) {
                documents.push($(this).val());
            }
        });
        if (documents.length == 0 || $('.Email').val() == "") {
            if ($('.Email').val() == "")
                $('#spn_email_error').html("Please enter email id.").show();
            if (documents.length == 0)
                $('#spn_email_error').html("Please select at least one document to email.").show();
            $('#overlay').hide();
            return false;
        }
        else {
            $('#spn_email_error').hide();
            $('#spn_error').html("").hide();
        }
        if (validateEmail($('.Email').val())) {
            $.ajax({
                type: "POST",
                url: '../Home/SendToEmail',
                data: {
                    "productId": $('#hdn_ProductId').val(),
                    "email": $('.Email').val(),
                    "documents": documents
                },
                success: function (data) {
                    $('#spn_error').hide();
                    $('#overlay').hide();
                    $('#spn_success').html("Regulatory documents sent to email " + $('.Email').val()).show();
                },
                error: function (error) {
                    $('#overlay').hide();
                    var obj = JSON.parse(JSON.stringify(error));
                    $('#spn_error').html(obj.responseJSON.detail).show();
                }
            });
        }
    });

    function validateEmail(email) {
        $.ajax({
            url: '../Home/IsValidEmail',
            data: {
                "email": email,
            },
            success: function (data) {
                if (data.toLowerCase() == "true") {
                    $('#spn_email_error').html("").hide()
                    return true;
                }
                else {
                    $('.Email').focus();
                    $('#spn_email_error').html("Please enter a valid email.").show();
                    $('#overlay').hide();
                    return false;
                }
            },
            error: function (error) {
                $('.Email').focus();
                $('#spn_error').html(error).show();
                return false;
            }
        });
    }

    $(".tabs__nav_link").click(function () {
        clearmessages();
        $("#chk_selectall").prop('checked', false);
        $('.chk_document').each(function (i, obj) {
            $(this).prop('checked', false);
        });
        $(".tabs__nav_link").removeClass('is__active');
        var tab_id = "#" + $(this).attr("id");
        $(tab_id).addClass('is__active');
        $(".tab-wrapper-content div").removeClass('is__active');
        $(tab_id + ".tabs__content").addClass('is__active');
        var id = "." + $(this).attr('id');
        $('.all').hide();
        $(id).show();
    });
});