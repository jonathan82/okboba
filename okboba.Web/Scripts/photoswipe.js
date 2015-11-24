/*
 *  Plugin : Photoswipe jQuery wrapper
 *  Author : Jonathan Lin
 *  Date   : 11/19/2015
 *  Notes  : 
 */

(function ($) {

    //Initialize plugin. called on an img tag
    $.fn.photoswipe = function () {

        this.click(function () {

            var slides = [];

            var opts = {
                bgOpacity: 0.8,
                getThumbBoundsFn: function (index) {                    
                    var $img = $('img[src="' + slides[index].msrc + '"]');
                    return {
                        x: $img.offset().left,
                        y: $img.offset().top,
                        w: $img.width()
                    };
                }
            }

            //get the current src
            var currSrc = $(this).prop('src');

            //build an array of slides to pass to pswp
            $('.photo-thumbnail-full').each(function (index) {
                slides.push({
                    src: $(this).data('original'),
                    msrc: $(this).prop('src'),
                    w: $(this).data('w'),
                    h: $(this).data('h')
                });
                if (currSrc == $(this).prop('src')) opts.index = index;
            });

            //create the gallery
            var template = $('.pswp').get(0);
            var gallery = new PhotoSwipe(template, PhotoSwipeUI_Default, slides,opts);
            gallery.init();
        });
    }

})(jQuery);