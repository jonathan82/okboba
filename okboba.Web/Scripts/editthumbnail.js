/*
 *  Plugin : Edit Thumbnail
 *  Author : Jonathan Lin
 *  Date   : 11/19/2015
 *  Notes  : 
 */

(function ($) {

    //Initialize plugin
    $.fn.editthumbnail = function () {

        var $modal;

        function DismissHandler() {
            //remove the modal
            $modal.remove();
        }

        //setup handler to activate modal
        this.click(function () {
            //get data to pass to template
            var data = {
                photo: $(this).data('photo')
            }

            //create template and insert in DOM
            var tmpl = $.templates('#editThumbnailTemplate');
            var html = tmpl.render(data);
            $('body').append(html);
            $modal = $('#editThumbnailModal');

            //show spinner and handler for imagesLoaded
            $modal.find('.modal-body').spin(spinOpts);
            
            //show modal            
            $modal.modal();

            //modal event handlers
            $modal.on('hidden.bs.modal', DismissHandler);
        });

    }

})(jQuery);