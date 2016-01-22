/*
 *  Plugin : Adds a vertically and horizontally centered boba loading icon to the selected div.
 *  Author : Jonathan Lin
 *  Date   : 1/19/2016
 *  Notes  : 
 */
(function ($) {
    /////////////////// Module vars /////////////////////
    var configMap = {
        bobaIcon: '/content/images/boba-loading.gif',
        bobaIconSize: 150 //size of loading icon in px, assumes square
    }

    ////////////////// Private Methods ///////////////////
    function createBoba() {
        var div;
        div = $('<div class="boba-loading"><img/></div>');
        div.css({ position: 'absolute', top: '50%', left: '50%' });
        div.css('margin-top', '-75px');
        div.css('margin-left', '-75px');
        div.children('img').width(configMap.bobaIconSize);
        div.children('img').prop('src', configMap.bobaIcon);
        return div;
    }

    ///////////////// Public API //////////////////////
    $.fn.bobaloader = function (action) {
        var boba;

        if (action == 'start') {
            boba = createBoba();
            $(this).css({ position: 'relative' });
            $(this).append(boba);
        }

        if (action == 'stop') {
            $(this).children('.boba-loading').remove();
        }
        
    }

})(jQuery);