/*
 *  Utitlity Functions
 * 
 */
function encodeHtml(str) {
    return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

/*
 *  Plugin : Photo Upload
 *  Author : Jonathan Lin
 *  Date   : 10/15/2015
 *  Notes  : Requires FileReaderJS library and jQuery UI.
 * 
 */
(function ($) {

    const MAX_FILE_SIZE = 5000000;
    const MIN_RESOLUTION = 200;
    const MAX_SCREEN_WIDTH = 800;

    var photoArea;

    // Options for thumbnail selector
    var dragOpts = {
        containment: 'parent'

    }
    var resizeOpts = {
        containment: 'parent',
        minWidth: 100,
        minHeight: 100,
        aspectRatio: true
    }

    // Options for FilereaderJS
    var opts = {
        on: {
            load: function (e, file) {
                //Check Size
                if (file.size > MAX_FILE_SIZE) {
                    alert('Upload file size must be less than 5MB!');
                    return;
                }

                //Check Type
                if (!file.type.match(/image.*/)) {
                    alert('You must choose an image file!');
                    return;
                }

                //Check minimum resolution
                var $img = $('<img id=""/>')
                    .attr("src", e.target.result)
                    .load(function () {
                        if (this.width < MIN_RESOLUTION || this.height < MIN_RESOLUTION) {
                            //Image too small
                            alert('Photo must be at leat 200 x 200 pixels!');
                            return;
                        }

                        //Everythign OK, create thumbnail selector and add to DOM
                        if (this.width > MAX_SCREEN_WIDTH) {
                            $(this).width(MAX_SCREEN_WIDTH);
                        }

                        var $thumbDiv = $('<div class="photo-upload-container"> \
                        <div class="photo-upload-innercontainer"><div class="photo-thumbnail-selector"></div></div> \
                        </div>');

                        $thumbDiv.append($img);
                        $(photoArea).after($thumbDiv);
                        $(photoArea).hide();

                        //Setup the thumbail selector to be resizeable and draggeable
                        $('.photo-thumbnail-selector').draggable(dragOpts).resizable(resizeOpts);
                    });
            },
            error: function (e, file) {
                alert('error');
            }
        }
    }

    // Initialize the plugin for a modal
    $.fn.photoUpload = function () {

        photoArea = this.data('target');

        this.find('input[type="file"]').fileReaderJS(opts);

        //Setup the dismiss event handler
        this.on('hidden.bs.modal', function (e) {

            if ($(photoArea).next().hasClass('photo-upload-container')) {
                $(photoArea).next().remove();
            }

            $(photoArea).show();
        });

        //Setup the Save handler -- do the real work
        $('[data-submit="modal"]').click(function (e) {

            // Make sure an image is selected first
            var $thumb = $('.photo-thumbnail-selector');
            if ($thumb.length == 0) {
                alert('You must select an image first!');
                return;
            }

            var pos = $thumb.position();

            // Set the thumbnail values in the form
            $('#topThumb').val(pos.top);
            $('#leftThumb').val(pos.left);
            $('#widthThumb').val($thumb.width());

            alert("submitting form. \
                   \nleft: " + pos.left +
                   '\ntop: ' + pos.top +
                   '\nwidth: ' + $thumb.width());
        });
    }

})(jQuery);

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
            var $newElement = $('<form><textarea class="profile-text-edit" id=qText-"'+id+'" name="qText">' + txt + '</textarea> \
                                 <button type="button" class="btn btn-default">Cancel</button> \
                                 <button type="button" class="btn btn-primary">Save</button> \
                                 <input type="hidden" name="whichQuestion" value="'+id+'" /></form>');

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

