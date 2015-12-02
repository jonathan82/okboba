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

    var url = '/location/getdistricts';

    // Where to position the location picker
    var opt = {
        my: 'left top',
        at: 'left bottom',
        of: null,
        collision: 'none'
    }

    function BuildData(locations) {
        var data = {
            first: [],
            second: [],
            third: [],
            fourth: []
        }

        //Build the alphabetically sorted data
        for (var i = 0; i < locations.length; i++) {
            var str = locations[i].Pinyin;
            var c = str.charAt(0);
            if ('a' <= c && c <= 'g') {
                data.first.push(locations[i]);
            } else if ('h' <= c && c <= 'l') {
                data.second.push(locations[i]);
            } else if ('m' <= c && c <= 's') {
                data.third.push(locations[i]);
            } else if ('t' <= c && c <= 'z') {
                data.fourth.push(locations[i]);
            }
        }

        return data;
    }

    function Find(ids, data) {
        // Finds the given ids in the data array and returns the locations 
        // matching them.
        var ret = [];
        for (var i = 0; i < data.length; i++) {
            if ($.inArray(data[i].LocationId, ids) >= 0) ret.push(data[i]);
        }
        return ret;
    }

    function Finish(loc1, loc2) {
        var $tb = $('#locationinput');
        $tb.val(loc2.LocationName + ', ' + loc1.LocationName);
        $('locationId1').val(loc1.LocationId);
        $('locationId2').val(loc2.LocationId);
        $tb.trigger('locationpicker:close');
    }

    function Step2 (current, all, picker) {
        //load district data
        $.getJSON(url, { provinceId: current.LocationId }, function (districts) {
            var data = BuildData(districts);
            data.current = current;

            //load template
            var tmpl = $.templates('#locationPickerTemplate');
            picker.html(tmpl.render(data));

            //step 1 click handler
            picker.find('a[data-step="1"]').click(function (e) {
                e.preventDefault();
                //load step 1
                Step1(all, picker);
            });

            //finish click handler
            picker.find('a[data-id]').click(function (e) {
                e.preventDefault();
                var id = $(this).data('id');
                var loc2 = Find([id], districts);
                Finish(current, loc2[0]);
            });

        }).fail(function () {
            alert('failed to load district data');
        });
    }

    function Step1(all, picker) {
        //build the data
        var data = BuildData(all);
        data.hot = Find([2, 27, 29], all);

        //load template
        var tmpl = $.templates('#locationPickerTemplate');
        picker.html(tmpl.render(data));
        
        //setup event handlers
        picker.find('a').click(function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            var current = Find([id], all);
            Step2(current[0], all, picker);
        });
    }

    $.fn.locationpicker = function (prov) {

        var $picker;
        var $textbox = $(this);
        var open = false;

        //Open the location picker
        this.focus(function () {
            if (open) return;
            open = true;

            //load and position the location picker
            $picker = $('<div class="location-picker-container"></div>');
            $(this).after($picker);
            opt.of = this;
            $picker.position(opt);

            Step1(prov, $picker);
        });

        $textbox.on('locationpicker:close', function () {
            if (!open) return;
            //close the picker
            $picker.remove();
            open = false;
        });

        //Setup the click dismiss handler
        $(document).click(function (e) {
            if (!open) return;

            var top = $picker.offset().top,
                left = $picker.offset().left,
                width = $picker.width(),
                height = $picker.height();

            if (left <= e.pageX && e.pageX <= left + width &&
                top <= e.pageY && e.pageY <= top + height) {
                $textbox.focus();
                return;
            }

            if ($(e.target).is($textbox)) return;
            
            //close the picker
            $textbox.trigger('locationpicker:close');
        });

        //setup the tab and esc handlers
        $textbox.keydown(function (e) {
            if(e.keyCode == 9 || //tab
                e.keyCode == 27) { //escape
                $textbox.trigger('locationpicker:close');
            }
        });
    }

})(jQuery);