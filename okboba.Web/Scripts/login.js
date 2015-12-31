/*
 *  Author : Jonathan Lin
 *  Date   : 12/30/2015
 *  Notes  : Plugin for handling Login validation and other login logic.
 */
(function ($) {

    //// Private Variables
    var configMap = {
        loginUrl: '/account/login',
        returnUrl: '/home',
        loginFormSel: '#loginForm',
        loginErrMsg: ''
    }

    //Validation options for login form
    var opt = {
        rules: {
            Email: "required",
            Password: "required"
        },
        submitHandler: Submit
    }

    //// Pivate Methods
    function Success() {
        //load return url
        window.location.replace(configMap.returnUrl);
    }

    function Submit(form) {

        //this will be set to the validator object
        var validator = this;

        validator.resetForm();

        //disable submit to prevent double submit
        $(form).find('button[type="submit"]').prop("disabled", true);

        $.post(configMap.loginUrl, $(form).serialize(), Success)
            .fail(function () {
                //show login error - show a generic message on both username and password for security reasons
                validator.showErrors({ "Password": configMap.loginErrMsg });
                $(form).find('button[type="submit"]').prop("disabled", false);
            });
    }

    //// Plugin initialization (exposed thru jQuery)
    // Called on the login modal
    $.fn.login = function (config) {

        configMap = $.extend(configMap, config);

        //reset form when modal is hidden
        this.on('hidden.bs.modal', function () {

        });

        //prevent default form submission since we're doing ajax submit
        this.find(configMap.loginFormSel).submit(function (e) {
            e.preventDefault();
        })
            .validate(opt);
    }

})(jQuery);