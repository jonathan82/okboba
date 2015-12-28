/*
 *  Plugin : Edit in place for OkBoba
 *  Author : Jonathan Lin
 *  Date   : 10/13/2015
 *  Notes  : 
 * 
 */
(function ($) {

    // The URL to post to
    var url = '/profile/editprofiletext';

    // Initialize the plugin. this refers to the edit icon with data-target = id of div
    // containing the profile text
    $.fn.editinplace = function () {
        
        var editTemplate = $.templates('#editTextTemplate');

        this.click(function (e) {

            //Get the targets            
            var $qDiv = $($(this).data('target'));
            var $editIcon = $(this);
            var $newForm;

            ///////////////////// Handlers ///////////////////////////
            function SaveHandler(e) {
                e.preventDefault();
                var postData = $newForm.serialize();

                //disable buttons to prevent double submit
                $newForm.children('button').prop('disabled', true);

                $.post(url, postData, function (data) {
                    //successfully posted data
                    //show posted text and edit icon, remove form, remove placeholder class
                    var newTxt = $newForm.children('textarea:first').val();
                    $qDiv.html(encodeHtml(newTxt));
                    $qDiv.removeClass('profile-text-placeholder');
                    $qDiv.show();
                    $editIcon.show();
                    $newForm.remove();

                }).fail(function () {
                    alert('error submitting profile text');
                    //renable buttons
                    $newForm.children('button').prop('disabled', false);
                });
            }

            function CancelHandler(e) {
                //show original text, edit icon, placeholder
                //remove form
                $qDiv.show();
                $editIcon.show();
                $newForm.remove();
            }            

            //Grab the html to put in the textarea
            var html = $qDiv.hasClass('profile-text-placeholder') ? '' : $qDiv.html();

            //$newForm = $('<form><textarea class="profile-text-edit form-control" id=qText-"' + $qDiv.prop('id') + '" name="text">' + br2nl(html) + '</textarea> \
            //              <button type="button" class="btn btn-default">Cancel</button> \
            //              <button type="button" class="btn btn-primary">Save</button> \
            //              <input type="hidden" name="whichQuestion" value="' + $qDiv.prop('id') + '" /></form>');
            $newForm = $(editTemplate.render({
                id: $qDiv.prop('id'),
                text: br2nl(html)
            }));

            $qDiv.after($newForm);

            //Hide the original text and edit icon
            $qDiv.hide();
            $editIcon.hide();

            //Setup the event handlers for the form
            $newForm.find('.btn-primary').click(SaveHandler);
            $newForm.find('.btn-default').click(CancelHandler);
        });

        //// Attach click handler to all the edit icons
        //$('.js-editinplace-editicon').click(function (e) {

        //    var target = $(this).data('target');
        //    var $textDiv = $(target);

        //    var txt = $.trim($textDiv.text());
        //    var id = $textDiv.prop('id');

        //    //Replace the text with the form
        //    var $newElement = $('<form><textarea class="profile-text-edit" id=qText-"' + id + '" name="qText">' + txt + '</textarea> \
        //                         <button type="button" class="btn btn-default">Cancel</button> \
        //                         <button type="button" class="btn btn-primary">Save</button> \
        //                         <input type="hidden" name="whichQuestion" value="'+ id + '" /></form>');

        //    $textDiv.after($newElement);

        //    var $savedElement = $textDiv.detach();

        //    //Setup event handler for the Save button
        //    $newElement.children('button:nth-of-type(2)').click(function (e) {

        //        e.preventDefault();

        //        var postData = $newElement.serialize();

        //        //Disable save button
        //        var $submitBtn = $(this);
        //        $submitBtn.prop("disabled", true);

        //        //Submit the question text
        //        $.post(url, postData, function (data) {

        //            //Replace form with the new text
        //            var txt = $newElement.children('textarea:first').val();
        //            $savedElement.text(txt);
        //            $newElement.replaceWith($savedElement);

        //        }).fail(function (data, status) {
        //            alert('failed! - ' + status);

        //            //Re-enable save button
        //            $submitBtn.prop("disabled", false);
        //        });

        //    });

        //    //setup event handler for the Cancel button
        //    $newElement.children('button:first').click(function (e) {
        //        $newElement.replaceWith($savedElement);
        //        return false;
        //    });

        //});

        return this;
    }

})(jQuery);

