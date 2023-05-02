function isEmail() {
    if ($('.Email').val() == "") {
        $('#spn_email_error').html("Please enter email.").show();
        return false;
    }
    return validateEmail($('.Email').val());
}
function clearmessages() {
    $('#spn_dl_error').html("").hide();
    $('#spn_email_error').html("").hide();
    $('#spn_error').html("").hide();
    $('#spn_success').html("").hide();

};
function validateEmail(email) {
    clearmessages();
    if (email == "") {
        $('#spn_email_error').html("Please enter email.").show();
        return false;
    }
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
function Validate() {
    clearmessages();
    var documents = [];
    $('.chk_document').each(function (i, obj) {
        if ($(this).is(':checked')) {
            documents.push($(this).val());
        }
    });

    if (documents.length == 0) {
        $('#spn_dl_error').html("Please select at least one document to download").show();
        return false;
    }
    else {
        $('#spn_dl_error').html("").hide();
        ShowLoader();
    }

};

function ValidateProductCode() {
    if ($('#ProductCode').val() == "") {
        $('#spn_error').html("Please enter a product code to search.").show();
        return false;
    }
    else {
        $('#spn_error').html("").hide();
    }
    $('#overlay').show();
    setTimeout(function () { $('#overlay').hide() }, 5000);
    return true;
};

function ShowLoader() {
    $('#overlay').show();
    setTimeout(function () { $('#overlay').hide() }, 5000);
    return true;
};