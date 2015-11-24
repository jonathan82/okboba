/*
 *  Plugin : Edit Thumbnail, Delete Photo
 *  Author : Jonathan Lin
 *  Date   : 11/19/2015
 *  Notes  : 
 */

(function ($) {

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

    //Initialize plugin
    $.fn.editthumbnail = function () {

        var $modal;
        var photo;

        function DismissHandler() {
            //remove the modal
            $modal.remove();
        }

        function SaveHandler() {
            //disable buttons
            $modal.find('button').prop('disabled', true);

            var $thumb = $modal.find('.photo-thumbnail-selector');
            var pos = $thumb.position();
            var imgWidth = $modal.find('.photo-on-screen').width();

            //send the on-screen width. let the server code figure out scaling
            $('#topThumb').val(pos.top);
            $('#leftThumb').val(pos.left);
            $('#widthThumb').val($thumb.width());
            $('#photoScreenWidth').val(imgWidth);
            $('#photo').val(photo);

            //alert(pos.top + ', ' + pos.left + ', ' + $thumb.width() + ', ' + imgWidth);            
        }

        //setup handler to activate modal
        this.click(function () {
            //get data to pass to template
            var data = {
                src: $(this).data('src')
            }

            //save the photo name
            photo = $(this).data('photo');

            //create template and insert in DOM
            var tmpl = $.templates('#editThumbnailTemplate');
            var html = tmpl.render(data);
            $('body').append(html);
            $modal = $('#editThumbnailModal');

            //show the thumbnail box only when images loaded
            $modal.find('.modal-body').spin('large');
            $modal.imagesLoaded(function () {

                $modal.find('.modal-body').spin(false);
                $('.photo-thumbnail-selector').draggable(dragOpts).resizable(resizeOpts);

            });            
            
            //show modal            
            $modal.modal();

            //modal event handlers
            $modal.on('hidden.bs.modal', DismissHandler);
            $modal.children('form').submit(SaveHandler);
        });

    }

    //Delete plugin
    $.fn.deletephoto = function () {

        this.click(function () {

            var res = confirm("Are you sure you want to delete this photo?");
            
            var photo = $(this).data('photo');

            if (res) {
                $.ajax('/photo/delete', {
                    method: 'POST',
                    traditional: true,
                    data: {photo: photo}
                }).done(function () {  
                    alert('delete successful');

                }).fail(function () {
                    alert('failed');
                });
            }

        });
    }

})(jQuery);