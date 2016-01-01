/*
 *  Author : Jonathan Lin
 *  Date   : 12/2/2015
 *  Notes  : Global Validation settings for okboba. Override jQuery Validation 
 *           methods to display Bootstrap error classes.
 * 
 *           See this link for hilight/unhilight error:
 * 
 *           http://stackoverflow.com/questions/22903707/jquery-validate-is-firing-both-highlight-and-unhighlight-in-chrome
 */
(function ($) {

    var opt = {
        errorElement: "span",
        errorClass: "help-block",

        //errorPlacement: function (error, element) {

        //},
        
        /*
         * Highlight: If there's an error we highlight it by 
         *  - adding the has-error class to the form-group container
         *  - adding an "X" glyphicon to the input control
         */
        highlight: function (element, errorClass) {
            console.log('hilight: ' + $(element).prop('name'));
            var fg = $(element).closest('.form-group');
            if (fg.hasClass('has-error')) {
                //if element is already highlighted we don't do anything. prevent multiple "X"'s from being added
                return;
            }
            $(element).closest('.form-group').addClass('has-error');
            $(element).closest('.form-group').addClass('has-feedback');
            $(element).after('<span class="glyphicon glyphicon-remove form-control-feedback"></span>');
        },

        /*
         * Unhighlight: we undo what highlight did
         */
        unhighlight: function (element, errorClass) {
            console.log('unhighlight: ' + $(element).prop('name'));
            $(element).closest('.form-group').removeClass('has-error');
            $(element).closest('.form-group').removeClass('has-feedback');
            $(element).nextAll('.glyphicon-remove').remove();
        },
        
    }

    jQuery.validator.setDefaults(opt);

})(jQuery);

