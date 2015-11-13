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
    var origImageWidth;
    var scaledImageWidth;

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

                        origImageWidth = scaledImageWidth = this.width;

                        // Limit the on screen size
                        if (this.width > MAX_SCREEN_WIDTH) {
                            $(this).width(MAX_SCREEN_WIDTH);
                            scaledImageWidth = MAX_SCREEN_WIDTH;
                        }

                        //Everythign OK, create thumbnail selector and add to DOM
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
            // Compute the scaled thumbnail coordinates base on original image
            var intTop = Math.round(origImageWidth / scaledImageWidth * pos.top);
            var intLeft = Math.round(origImageWidth / scaledImageWidth * pos.left);
            var intThumbWidth = Math.round(origImageWidth / scaledImageWidth * $thumb.width())

            $('#topThumb').val(intTop);
            $('#leftThumb').val(intLeft);
            $('#widthThumb').val(intThumbWidth);

            alert("submitting form. \
                   \nleft: " + intTop +
                   '\ntop: ' + intLeft +
                   '\nwidth: ' + intThumbWidth);
        });
    }

})(jQuery);
