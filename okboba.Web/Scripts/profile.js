/*
 *  Plugin : Profile functionlity including editing profile text.
 *  Author : Jonathan Lin
 *  Date   : 1/18/2016
 *  Notes  : 
 */
; (function ($) {
    ///////////// Module Vars ///////////////////
    var configMap = {
        editTextTemplateSel: '#editTextTemplate',
        editTextApi: '/profile/editprofiletext'
    }

    var editTextTemplate;

    ///////////// Private methods ///////////////
    function Save(form, target, editIcon) {
        var data, newText;

        newText = form.children('textarea:first').val().trim();

        //If input is empty do nothing
        if (newText == '') return;

        data = form.serialize();

        $.post(configMap.editTextApi, data, function (result) {

            $(target).html(nl2br(result));
            $(target).show();

            editIcon.show();

            form.remove();

        }).fail(function () {
            alert('failed');
        });
    }

    function Cancel(form, target, editIcon) {
        form.remove();
        $(target).show();
        editIcon.show();
    }

    ///////////// Public API ////////////////////
    // Exposed thru jquery. Called on the edit icon
    $.fn.editinplace = function () {

        //Load the template
        editTextTemplate = $.templates(configMap.editTextTemplateSel);

        //Setup the click handler
        this.click(function () {
            var target, originalText, form, editIcon;

            editIcon = $(this);

            target = editIcon.data('target');

            originalText = $(target).html();

            form = $(editTextTemplate.render({
                id: $(target).prop('id'),
                text: br2nl(originalText)
            }));

            $(target).hide();
            editIcon.hide();
            $(target).closest('.profile-text-box').append(form);

            //Setup the event handlers for the form
            form.find('.btn-primary').click(function (e) { Save(form, target, editIcon); });
            form.find('.btn-default').click(function (e) { Cancel(form, target, editIcon); });
        });
    }

})(jQuery);