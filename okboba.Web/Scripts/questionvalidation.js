/*
 *  Plugin : Contains the validation logic for the question box.  Includes Importance Bar
 *           selector, validation, and handle Irrelevant flag.  Logic for question submission
 *           is contained in another plugin. jQuery plugin should be called on the question-form
 *           div.
 * 
 *  Author : Jonathan Lin
 *  Date   : 12/20/2015
 */

(function ($) {

    //// Private Funcions
    function highlightImportanceBar(level,form) {

        //clear highlights
        form.find('.question-importance-bar').removeClass('active');

        switch (level) {
            case 3:
                form.find('[data-importance="3"]').addClass('active');
            case 2:
                form.find('[data-importance="2"]').addClass('active');
            case 1:
                form.find('[data-importance="1"]').addClass('active');
                break;
            default:
                //do nothing.  highlights already cleared
        }
    }

    function setupImportanceBars(form) {

        //Setup the mouse enter handler
        form.find('.question-importance-bar').mouseenter(function () {
            var level = $(this).data('importance');
            highlightImportanceBar(level,form);
        });

        //Setup the mouse leave handler
        form.find('.question-importance-bar').mouseleave(function () {
            //restore the saved importance value from the hidden input
            var savedLevel = form.find('input[name="ChoiceImportance"]').val();
            savedLevel = parseInt(savedLevel);
            highlightImportanceBar(savedLevel,form);
        });

        //Setup the click handler
        form.find('.question-importance-bar').click(function () {
            var level = $(this).data('importance');
            form.find('input[name="ChoiceImportance"]').val(level);
            validate(form);
        });
    }

    function setupCheckboxes(form) {

        //Other checkboxes click
        form.find('input[name="ChoiceAccept"]').change(function () {
            var checked = true;

            //loop all the "other" checkboxes to set the irrelevant flag
            form.find('input[name="ChoiceAccept"]').each(function () {
                checked = checked && $(this).prop('checked');
            });

            form.find('input[name="ChoiceIrrelevant"]').prop('checked', checked);

            validate(form);
        });

        //Irrelevant clicked
        form.find('input[name="ChoiceIrrelevant"]').change(function () {
            var checked = $(this).prop('checked');
            form.find('input[name="ChoiceAccept"]').prop('checked', checked);
            validate(form);
        });
    }

    function setupRadios(form) {
        form.find('input:radio').change(function () {
            validate(form);
        });
    }

    function validate(form) {
        var radioChecked,
            checkboxChecked,
            irrelevantFlag,
            importance,
            isValid = true;

        //choice required - at least one radio checked
        radioChecked = form.find('input:radio').is(':checked');

        //at least one checkbox required
        checkboxChecked = form.find('input:checkbox').is(':checked');

        //show/hide importance bars depending irrelevant flag
        irrelevantFlag = form.find('input[name="ChoiceIrrelevant"]').is(':checked');
        if (irrelevantFlag) {
            form.find('.question-importance-box').hide();
        } else {
            form.find('.question-importance-box').show();
        }

        //get the importance value
        importance = form.find('input[name="ChoiceImportance"]').val();
        
        //Set Validation flag
        if (!radioChecked) isValid = false;
        if (!checkboxChecked) isValid = false;
        if (!irrelevantFlag && importance == 0) {
            isValid = false;
        }

        //Enable/disable answer button based on validation flag
        form.find('button.btn-primary').prop('disabled', !isValid);
    }

    //// Public Functions (exposed thru jQuery)
    $.fn.questionValidation = function () {

        //check if we actually have a question form
        if (this.length == 0) return;

        setupImportanceBars(this);
        setupCheckboxes(this);
        setupRadios(this);

        return this;
    }

})(jQuery);