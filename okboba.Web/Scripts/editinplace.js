/*
 *  Plugin : Edit in place for OkBoba
 *  Author : Jonathan Lin
 *  Date   : 10/13/2015
 *  Notes  : 
 * 
 */
(function ($) {

    // Initialize the plugin
    $.fn.editinplace = function () {

        // The URL to post to
        var url = '/profile/editprofiletext';

        // Attach click handler to all divs with data-editinplace attribute
        $('div[data-editinplace]').click(function (e) {

            var $this = $(this);

            var txt = $.trim($this.text());
            var id = $this.data('editinplace');

            //Replace the text with the form
            var $newElement = $('<form><textarea class="profile-text-edit" id=qText-"' + id + '" name="qText">' + txt + '</textarea> \
                                 <button type="button" class="btn btn-default">Cancel</button> \
                                 <button type="button" class="btn btn-primary">Save</button> \
                                 <input type="hidden" name="whichQuestion" value="'+ id + '" /></form>');

            $this.after($newElement);

            var $savedElement = $this.detach();

            //Setup event handler for the Save button
            $newElement.children('button:nth-of-type(2)').click(function (e) {

                e.preventDefault();

                var postData = $newElement.serialize();

                alert('save me! - ' + postData);

                //Disable save button
                var $submitBtn = $(this);
                $submitBtn.prop("disabled", true);

                //Submit the question text
                $.post(url, postData, function (data) {

                    //Replace form with the new text
                    var txt = $newElement.children('textarea:first').val();
                    $savedElement.text(txt);
                    $newElement.replaceWith($savedElement);

                }).fail(function (data, status) {
                    alert('failed! - ' + status);

                    //Re-enable save button
                    $submitBtn.prop("disabled", false);
                });

            });

            //setup event handler for the Cancel button
            $newElement.children('button:first').click(function (e) {
                $newElement.replaceWith($savedElement);
                return false;
            });

        });

        return this;
    }

})(jQuery);

