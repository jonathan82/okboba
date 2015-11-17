/*
 *  Plugin : Photo Upload
 *  Author : Jonathan Lin
 *  Date   : 10/15/2015
 *  Notes  : Requires FileReaderJS library and jQuery UI.
 * 
 */
(function ($) {

    const MAX_FILE_SIZE = 5000000; //5 MB
    const MIN_RESOLUTION = 300;
    const MAX_SCREEN_WIDTH = 800;

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
                            alert('Photo must be at least ' + MIN_RESOLUTION + ' pixels!');
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
                        $('#photoUploadArea').after($thumbDiv);
                        $('#photoUploadArea').hide(); //hide so we don't get rid of file input

                        //Setup the thumbail selector to be resizeable and draggeable
                        $('.photo-thumbnail-selector').draggable(dragOpts).resizable(resizeOpts);
                    });
            },
            error: function (e, file) {
                alert('error');
            }
        }
    }

    // Initialize the plugin for a modal. Assume being called on a button
    $.fn.photoUpload = function () {

        var $modal;

        //Setup the click handler to create modal
        this.click(function (e) {
            var tmpl = $.templates('#photoUploadTemplate');
            $('body').append(tmpl.render());
            $modal = $('#photoUploadModal');
            $modal.find('[data-submit="modal"]').click(SaveHandler);
            $modal.on('hidden.bs.modal', DismissHandler);

            var $fileInput = $modal.find('input[type="file"]');
            $fileInput.fileReaderJS(opts);

            $modal.modal();
        });

        function DismissHandler(e) {
            //remove modal from the page
            $modal.remove();
        }

        function SaveHandler (e) {

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
        }
    }

})(jQuery);
