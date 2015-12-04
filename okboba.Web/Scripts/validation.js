/*
 *  Author : Jonathan Lin
 *  Date   : 12/2/2015
 *  Notes  : Set the defaults for jQuery Validation plugin to make it work with Bootstrap.
 */
(function ($) {

    var opt = {
        errorElement: "span",
        errorClass: "help-block",

        //errorPlacement: function (error, element) {

        //},
        
        highlight: function (element, errorClass) {
            $(element).closest('.form-group').addClass('has-error');
            $(element).closest('.form-group').addClass('has-feedback');
            $(element).after('<span class="glyphicon glyphicon-remove form-control-feedback"></span>');
        },

        unhighlight: function (element, errorClass) {
            $(element).closest('.form-group').removeClass('has-error');
            $(element).closest('.form-group').removeClass('has-feedback');
            $(element).nextAll('.glyphicon-remove').remove();
        }
    }

    jQuery.validator.setDefaults(opt);

})(jQuery);

