/*
--------------------------------
Ajax Contact Form
--------------------------------
*/

(function ($, window, document, undefined) {
    'use strict';
    var form;

    $(document).ready(function () {
        $('.form-ajax').each(function (index) {
            form = this;
            var sendToEmail = $(this).attr("data-email");
            var engine = $(this).attr("data-engine");
            var grecaptcha_token = "";
            var subject = $(this).attr("data-subject");
            if (isEmpty(engine)) engine = "php";
            if (isEmpty(sendToEmail)) sendToEmail = '';
            if (isEmpty(subject)) subject = '';

            $(form).submit(function (e) {
                $('.form-group').removeClass('has-error');
                $('.help-block').remove();

                // Google reCaptcha
                if (typeof grecaptcha !== 'undefined') {
                    var client_key = $('#recaptcha').attr('src');
                    client_key = client_key.substring(client_key.indexOf('=') + 1, client_key.length);
                    grecaptcha.ready(function () {
                        grecaptcha.execute(client_key, { action: 'homepage' }).then(function (token) {
                            grecaptcha_token = token;
                            send_message(sendToEmail, subject, engine, grecaptcha_token);
                        });
                    });
                } else {
                    send_message(sendToEmail, subject, engine, "");
                }
                $(form).find(".cf-loader").show();
                e.preventDefault();
            });
        });
    });
    function send_message(sendToEmail, subject, engine, grecaptcha_token) {
        var formData = {
            'values': {},
            'domain': window.location.hostname.replace("www.", ""),
            'email': sendToEmail,
            'subject_email': subject,
            'engine': engine,
            'grecaptcha': grecaptcha_token
        };

        $(form).find("input,textarea,select").each(function () {
            var val = $(this).val();
            if (!isEmpty(val)) {
                var name = $(this).attr("data-name");
                if (isEmpty(name)) name = $(this).attr("name");
                if (isEmpty(name)) name = $(this).attr("id");
                var error_msg = $(this).attr("data-error");
                if (isEmpty(error_msg)) error_msg = "";
                formData['values'][name] = [val, error_msg];
            }
        });

        $.ajax({
            type: 'POST',
            url: $(form).attr("action"),
            data: formData,
            dataType: 'json',
            encode: true
        }).done(function (data) {
            if (!data.success) {
                // Error
                console.log(data);
                $(form).find(".error-box").show();
            } else {
                // Success
                $(form).html($(form).find(".success-box").html());
            }
            $(form).find(".cf-loader").hide();
        }).fail(function (data) {
            // Error
            console.log(data);
            $(form).find(".error-box").show();
            $(form).find(".cf-loader").hide();
        });
    }
}(jQuery, window, document));