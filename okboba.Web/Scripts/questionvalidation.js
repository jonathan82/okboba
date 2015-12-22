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
    //// Private vars
    var $questionForm;

    //// Private Funcions
    function highlightImportanceBar(level) {

        //clear highlights
        $questionForm.find('.question-importance-bar').removeClass('active');

        switch (level) {
            case 3:
                $questionForm.find('[data-importance="3"]').addClass('active');
            case 2:
                $questionForm.find('[data-importance="2"]').addClass('active');
            case 1:
                $questionForm.find('[data-importance="1"]').addClass('active');
                break;
            default:
                //do nothing.  highlights already cleared
        }
    }

    function setupImportanceBars() {

        //Setup the mouse enter handler
        $questionForm.find('.question-importance-bar').mouseenter(function () {
            var level = $(this).data('importance');
            highlightImportanceBar(level);
        });

        //Setup the mouse leave handler
        $questionForm.find('.question-importance-bar').mouseleave(function () {
            //restore the saved importance value from the hidden input
            var savedLevel = $questionForm.find('input[name="ChoiceImportance"]').val();
            savedLevel = parseInt(savedLevel);
            highlightImportanceBar(savedLevel);
        });

        //Setup the click handler
        $questionForm.find('.question-importance-bar').click(function () {
            var level = $(this).data('importance');
            $questionForm.find('input[name="ChoiceImportance"]').val(level);
            validate();
        });
    }

    function setupCheckboxes() {

        //Other checkboxes click
        $questionForm.find('input[name="ChoiceAccept"]').change(function () {
            var checked = true;

            //loop all the "other" checkboxes to set the irrelevant flag
            $questionForm.find('input[name="ChoiceAccept"]').each(function () {
                checked = checked && $(this).prop('checked');
            });

            $questionForm.find('input[name="ChoiceIrrelevant"]').prop('checked', checked);

            validate();
        });

        //Irrelevant clicked
        $questionForm.find('input[name="ChoiceIrrelevant"]').change(function () {
            var checked = $(this).prop('checked');
            $questionForm.find('input[name="ChoiceAccept"]').prop('checked', checked);
            validate();
        });
    }

    function setupRadios() {
        $questionForm.find('input:radio').change(function () {
            validate();
        });
    }

    function validate() {
        var radioChecked,
            checkboxChecked,
            irrelevantFlag,
            importance,
            isValid = true;

        //choice required - at least one radio checked
        radioChecked = $questionForm.find('input:radio').is(':checked');

        //at least one checkbox required
        checkboxChecked = $questionForm.find('input:checkbox').is(':checked');

        //show/hide importance bars depending irrelevant flag
        irrelevantFlag = $questionForm.find('input[name="ChoiceIrrelevant"]').is(':checked');
        if (irrelevantFlag) {
            $questionForm.find('.question-importance-box').hide();
        } else {
            $questionForm.find('.question-importance-box').show();
        }

        //get the importance value
        importance = $questionForm.find('input[name="ChoiceImportance"]').val();
        
        //Set Validation flag
        if (!radioChecked) isValid = false;
        if (!checkboxChecked) isValid = false;
        if (!irrelevantFlag && importance == 0) {
            isValid = false;
        }

        //Enable/disable answer button based on validation flag
        $questionForm.find('button.btn-primary').prop('disabled', !isValid);
    }

    //// Public Functions (exposed thru jQuery)
    $.fn.questionValidation = function () {

        $questionForm = this;

        setupImportanceBars();
        setupCheckboxes();
        setupRadios();
    }

})(jQuery);