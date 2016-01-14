/*
 *  Author : Jonathan Lin
 *  Date   : 1/13/2016
 *  Notes  : Contains functions for favoriting a user
 */
(function ($) {
    ////////////// Module vars /////////////////////
    var configMap = {
        saveFavoriteApi: '/favorites/save',
        removeFavoriteApi: '/favorites/remove',
        favoriteClass: 'is-favorited'
    }

    /////////////// Private functions //////////////////////
    function isFavorited() {
        return $(this).hasClass(configMap.favoriteClass);
    }

    ////////// Public API exposed thru jquery /////////////////
    $.fn.favorite = function () {
        var favoriteId, button;

        favoriteId = $(this).data('id');
        button = $(this);

        $(this).click(function () {
            if ((isFavorited.bind(this))()) {
                //remove favorite
                $.post(configMap.removeFavoriteApi, { favoriteId: favoriteId }).done(function () {
                    button.removeClass(configMap.favoriteClass);
                }).fail(function () {
                    //fail silently
                    console.log('error: couldn\'t remove favorite');
                });
            }
            else {
                //add favorite
                $.post(configMap.saveFavoriteApi, { favoriteId: favoriteId }).done(function () {
                    button.addClass(configMap.favoriteClass);
                }).fail(function () {
                    //fail silently
                    console.log('error: couldn\'t save favorite');
                });
            }
        });        
    }

})(jQuery);