/*
 *  Plugin : Location picker
 *  Author : Jonathan Lin
 *  Date   : 10/20/2015
 *  Notes  : Shows a 2-level province/district location picker organized by pinyin.
 *           ABCDEFG
 *           HIJKL
 *           MNOPQRS
 *           TUVWXYZ
 */
(function ($) {

    // Where to position the location picker
    var opt = {
        my: 'left top',
        at: 'left bottom',
        of: null,
        collision: 'none'
    }

    $.fn.locationpicker = function (prov) {

        var $picker;

        this.focus(function () {
            alert('focused');

            var data = {
                first: [],
                second: [],
                third: [],
                fourth: []
            }

            //Build the province data
            for (var i = 0; i < prov.length; i++) {
                var str = prov[i].Pinyin;
                var c = str.charAt(0);
                if ('a' <= c && c <= 'g') {
                    data.first.push({ name: prov[i].LocationName, id: prov[i].LocationId });
                } else if ('h' <= c && c <= 'l') {
                    data.second.push({ name: prov[i].LocationName, id: prov[i].LocationId });
                } else if ('m' <= c && c <= 's') {
                    data.third.push({ name: prov[i].LocationName, id: prov[i].LocationId });
                } else if ('t' <= c && c <= 'z') {
                    data.fourth.push({ name: prov[i].LocationName, id: prov[i].LocationId });
                }
            }

            //load the template and show under textbox
            var tmpl = $.templates('#locationPickerTemplate');
            $(this).after(tmpl.render(data));
            $picker = $(this).next();
            opt.of = this;            
            $picker.position(opt);

            //setup the click handlers
            $picker.find('a').click(function (e) {
                e.preventDefault();             
            });
        });

        this.focusout(function () {            
            //$picker.remove();
        });

    }

})(jQuery);