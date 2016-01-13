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
        loginErrMsg: 'generic err message'
    }

    //Validation options for login form
    var opt = {
        rules: {
            Email: "required",
            Password: "required"
        }
    }

    var validator;

    function Login(e) {
        var form, loginButton;

        form = $(configMap.loginFormSel);
        loginButton = $(this);

        e.preventDefault();

        if (!form.valid()) return false;

        loginButton.ladda().ladda('start');
       
        $.post(configMap.loginUrl, form.serialize()).done(function () {

            //success
            window.location.replace(configMap.returnUrl);

        }).fail(function () {

            //error - show a generic message on both username and password for security reasons
            validator.showErrors({ "Password": configMap.loginErrMsg, "Email" : configMap.loginErrMsg });

            loginButton.ladda().ladda('stop');
        });
    }

    //// Plugin initialization (exposed thru jQuery)
    // Called on the login modal
    $.fn.login = function (config) {

        configMap = $.extend(configMap, config);

        //reset form when modal is hidden
        this.on('hidden.bs.modal', function () {
            
        });

        //login handler
        this.find('.login-button').click(Login);

        //setup validation
        validator = this.find('form').validate(opt);
    }

})(jQuery);